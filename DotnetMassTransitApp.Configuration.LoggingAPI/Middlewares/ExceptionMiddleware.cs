using MassTransit;
using Newtonsoft.Json;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.LoggingAPI.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(
    RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
    HttpContext httpContext, 
    ISendEndpointProvider sendEndpointProvider)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            CancellationToken cancellationToken = httpContext.RequestAborted;

            var serializedError = JsonConvert.SerializeObject(ex);

            await sendEndpointProvider.Send<SendErrorDetail>(
            new SendErrorDetail
            {
                Service = typeof(ExceptionMiddleware).Namespace ?? string.Empty,
                Exception = serializedError
            });

            await httpContext.Response.WriteAsync(ex.Message, cancellationToken);
        }
    }
}
