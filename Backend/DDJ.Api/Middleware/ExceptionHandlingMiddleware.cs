using System.Net;
using System.Text.Json;
using DDJ.Api.Common;
using DDJ.Domain.Exceptions;
using FluentValidation;

namespace DDJ.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            NotFoundException nfe =>
                (HttpStatusCode.NotFound, ApiResponse.Fail(nfe.Message)),

            ValidationException ve =>
                (HttpStatusCode.BadRequest, ApiResponse.Fail(
                    "Validation failed",
                    ve.Errors.Select(e => e.ErrorMessage))),

            DomainException de =>
                (HttpStatusCode.BadRequest, ApiResponse.Fail(de.Message)),

            UnauthorizedAccessException =>
                (HttpStatusCode.Unauthorized, ApiResponse.Fail("Unauthorized")),

            _ =>
                (HttpStatusCode.InternalServerError, ApiResponse.Fail("An unexpected error occurred"))
        };

        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
