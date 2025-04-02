using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using FileUploader.Api.Exceptions;

namespace FileUploader.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var userId = context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "anonymous";
        _logger.LogError(ex, "Unhandled exception occurred. Path: {Path}, Method: {Method}, User: {User}, Query: {Query}, Message: {msg}",
            context.Request.Path,
            context.Request.Method,
            userId,
            context.Request.QueryString,
            ex.Message);
        context.Response.ContentType = "application/json";
        
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred";
        
        if (ex is ApiException apiException)
        {
            statusCode = (HttpStatusCode)apiException.StatusCode;
            message = apiException.Message;
        }
        else if (ex is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Unauthorized;
            message = "Unauthorized";
        }

        context.Response.StatusCode = (int)statusCode;


        var response = new
        {
            message,
            details = _env.IsDevelopment() ? ex.ToString() : null
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}