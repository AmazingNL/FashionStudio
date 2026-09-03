using System.Collections.Concurrent;
using FashionStudio.Api.Data;
using FashionStudio.Api.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FashionStudio.Api.Tests.Integration
{
    // Diagnostic-only sink so failing integration tests can print the real server-side
    // exception (GlobalExceptionHandler's HTTP response body is deliberately generic for 500s).
    public class CapturingLoggerProvider : ILoggerProvider
    {
        public readonly ConcurrentQueue<string> Entries = new();

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(categoryName, Entries);

        public void Dispose() { }

        private class CapturingLogger : ILogger
        {
            private readonly string _category;
            private readonly ConcurrentQueue<string> _entries;
            public CapturingLogger(string category, ConcurrentQueue<string> entries)
            {
                _category = category;
                _entries = entries;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
            public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;
                _entries.Enqueue($"[{logLevel}] {_category}: {formatter(state, exception)}{(exception != null ? "\n" + exception : "")}");
            }
        }
    }

    // Never actually connects anywhere — the real EmailService would try a live SMTP
    // connection (WorkSpaceInvitationService.SendInvitationAsync fails hard if sending fails),
    // which has no place in a fast, offline-safe test suite.
    public class FakeEmailService : IEmailService
    {
        public Task<bool> SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default) =>
            Task.FromResult(true);
    }

    // Boots the real app (real controllers, real [Authorize]/JWT pipeline, real
    // GlobalExceptionHandler) against an isolated in-memory database instead of Postgres.
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public readonly string DatabaseName = Guid.NewGuid().ToString();
        public readonly CapturingLoggerProvider Logs = new();

        static CustomWebApplicationFactory()
        {
            // Program.cs reads Jwt:Key (and builds the Npgsql connection string) directly out
            // of builder.Configuration BEFORE builder.Build() runs. WebApplicationFactory's own
            // ConfigureAppConfiguration/ConfigureServices hooks are only applied as part of
            // Build() itself, so they run too late to affect that code — these values have to
            // already be real environment variables by the time the host is first constructed.
            Environment.SetEnvironmentVariable("Jwt__Key", "integration-test-signing-key-0123456789");
            Environment.SetEnvironmentVariable("Jwt__Issuer", "FashionStudio.Tests");
            Environment.SetEnvironmentVariable("Jwt__Audience", "FashionStudio.Tests");
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", "Host=unused;Database=unused;Username=unused;Password=unused");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging => logging.AddProvider(Logs));
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(DatabaseName));

                var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
                if (emailDescriptor != null) services.Remove(emailDescriptor);
                services.AddScoped<IEmailService, FakeEmailService>();
            });
        }
    }
}
