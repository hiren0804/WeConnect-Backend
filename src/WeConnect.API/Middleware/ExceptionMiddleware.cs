using System.Net;
using System.Text.Json;
using WeConnect.Application.Common.Exceptions;
using WeConnect.Application.Common.Models;

namespace WeConnect.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(ctx, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        var (status, message, errors) = ex switch
        {
            NotFoundException     e => (HttpStatusCode.NotFound,       e.Message, null),
            ValidationException   e => (HttpStatusCode.UnprocessableEntity, e.Message,
                                          e.Errors.Select(x => new ApiError(x.Field, x.Error))),
            UnauthorizedException e => (HttpStatusCode.Unauthorized,   e.Message, null),
            ForbiddenException    e => (HttpStatusCode.Forbidden,      e.Message, null),
            ConflictException     e => (HttpStatusCode.Conflict,       e.Message, null),
            _                       => (HttpStatusCode.InternalServerError,
                                          "An unexpected error occurred.", null)
        };

        var traceId = ctx.TraceIdentifier;
        var response = new
        {
            success   = false,
            message,
            errors,
            traceId,
            timestamp = DateTime.UtcNow
        };

        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode  = (int)status;

        return ctx.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}