using System;
using System.IO;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FashionStudio.Api.Data
{
    // Provides EF Core tools a reliable way to construct AppDbContext at design time.
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // dotnet ef never runs Program.Main, so .env isn't loaded unless we do it here too.
            Env.Load();

            // Respect ASPNETCORE_ENVIRONMENT so environment-specific settings can be used.
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Build configuration from the project directory (where dotnet ef runs).
            var basePath = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            // Try connection string from configuration, then environment variable fallback.
            var connectionString = config.GetConnectionString("Default")
                                   ?? Environment.GetEnvironmentVariable("Default")
                                   ?? throw new InvalidOperationException(
                                       "Could not find a connection string named 'Default' in configuration or environment variables.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            // Use Npgsql provider; ensure the Npgsql EF Core package is referenced by the project.
            optionsBuilder.UseNpgsql(connectionString, b =>
                b.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
