using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Common.Exceptions;

namespace TodoList.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred at {TimeUtc}", DateTime.UtcNow);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ProblemDetails problemDetails;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails = new ValidationProblemDetails(validationEx.Errors)
                {
                    Title = "Validation Failed",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Status = statusCode
                };
                break;

            case NotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                problemDetails = new ProblemDetails
                {
                    Title = "Resource Not Found",
                    Detail = notFoundEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Status = statusCode
                };
                break;

            case ForbiddenAccessException forbiddenEx:
                statusCode = StatusCodes.Status403Forbidden;
                problemDetails = new ProblemDetails
                {
                    Title = "Forbidden",
                    Detail = forbiddenEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Status = statusCode
                };
                break;

            case UnauthorizedAccessException unauthorizedEx:
                statusCode = StatusCodes.Status401Unauthorized;
                problemDetails = new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = unauthorizedEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.2",
                    Status = statusCode
                };
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                problemDetails = new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Status = statusCode
                };
                break;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}
