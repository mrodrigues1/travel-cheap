using System.Net;
using System.Text.Json;
using FluentValidation;
using TravelCheap.Api.Infrastructure.Exceptions;

namespace TravelCheap.Api.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int) HttpStatusCode.BadRequest;
                response.Message = "One or more validation errors occurred.";

                response.Details = validationException.Errors.Select(x => new ValidationError
                {
                    PropertyName = x.PropertyName,
                    ErrorMessage = x.ErrorMessage
                });

                break;

            case NotFoundException notFoundException:
                response.StatusCode = (int) HttpStatusCode.NotFound;
                response.Message = notFoundException.Message;

                break;
            
            case BusinessLogicException businessLogicException:
                response.StatusCode = (int) HttpStatusCode.BadRequest;
                response.Message = businessLogicException.Message;

                break;

            default:
                response.StatusCode = (int) HttpStatusCode.InternalServerError;
                response.Message = "An internal error occurred.";

                break;
        }

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(
            response,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await context.Response.WriteAsync(jsonResponse);
    }
}

internal class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ValidationError
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
}
