using DotnetMassTransitApp.Configuration.LoggingAPI.Middlewares;

namespace DotnetMassTransitApp.Configuration.LoggingAPI.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void UseCustomExceptionMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
