using DotnetMassTransitApp.Configuration.LoggingAPI.Extensions;
using MassTransit;
using Shared.Queue.Contracts;
using Shared.Queue.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(mt =>
{
    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();
        cfg.Host(queueSettings.Uri, c =>
        {
            c.Username(queueSettings.Username);
            c.Password(queueSettings.Password);
        });

        cfg.Message<SendErrorDetail>(x =>
        {
            x.SetEntityName("send-error-detail-exchange");
        });

        EndpointConvention.Map<SendErrorDetail>(new Uri("exchange:send-error-detail-exchange?type=direct&routingKey=send-error-detail-routing-key"));

        cfg.ConfigureEndpoints(cntx);
    });
});

var app = builder.Build();

app.UseCustomExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
