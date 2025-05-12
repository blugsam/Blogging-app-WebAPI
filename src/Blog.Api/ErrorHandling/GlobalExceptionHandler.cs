using Blog.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Blog.Api.ErrorHandling
{
    public class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            logger.LogError(
                exception,
                "Exception occurred processing request {TraceId}. Method: {Method}, Path: {Path}, Message: {ErrorMessage}",
                traceId,
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message);

            var (statusCode, title, detail) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found", exception.Message),
                ConflictException => (StatusCodes.Status409Conflict, "Resource Conflict", exception.Message),
                BlogApplicationException => (StatusCodes.Status400BadRequest, "Application Rule Violation", exception.Message),

                // TODO: Добавить обработку UnauthorizedAccessException или других специфичных для .NET исключений, если нужно

                // TODO: Добавить обработку исключений аутентификации/авторизации, когда они будут реализованы
                // InvalidTokenException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
                // TokenExpiredException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
                // UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),

                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred. Please try again later.")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Type = GetErrorTypeUri(statusCode),
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };
            problemDetails.Extensions.Add("traceId", traceId);

            httpContext.Response.StatusCode = statusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
        }

        private static string GetErrorTypeUri(int statusCode) => statusCode switch
        {
            StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            StatusCodes.Status401Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            StatusCodes.Status409Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            StatusCodes.Status500InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _ => $"https://httpstatuses.com/{statusCode}"
        };
    }
}