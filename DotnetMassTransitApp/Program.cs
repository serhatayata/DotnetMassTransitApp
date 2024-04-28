using DotnetMassTransitApp.Models;
using MassTransit;
using Shared.Queue.Contracts;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<QueueSettings>("QueueSettings", configuration);

builder.Services.AddMassTransit(mt =>
{
    mt.SetKebabCaseEndpointNameFormatter();
    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();
        cfg.Host(queueSettings.Uri, c =>
        {
            c.Username(queueSettings.Username);
            c.Password(queueSettings.Password);
        });

        cfg.SendTopology.UseCorrelationId<SubmitOrder>(x => Guid.Parse(x.OrderId));
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
