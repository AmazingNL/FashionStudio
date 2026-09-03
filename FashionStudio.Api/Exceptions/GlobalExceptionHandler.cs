using Microsoft.AspNetCore.Diagnostics;

namespace FashionStudio.Api.Exceptions 
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
        {
            _logger.LogError(exception, "Unhandled exception");

            var (status, message) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                ConflictException => (StatusCodes.Status409Conflict, exception.Message),
                ValidationException => (StatusCodes.Status400BadRequest, exception.Message),
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred."),
            };

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(new { message }, ct);
            return true;
        }
    }
}
