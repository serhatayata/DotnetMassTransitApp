using DotnetMassTransitApp.Configuration.Topology.NameFormatters;
using MassTransit;
using Shared.Queue.Contracts;
using Shared.Queue.Events;
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

        cfg.Message<OrderSubmitted>(x =>
        {
            x.SetEntityNameFormatter(new FancyNameFormatter<OrderSubmitted>());
        });

        // We will get exchange:Shared.Queue.Contracts:ICommand => The message was not confirmed: NOT_FOUND - no exchange 'Shared.Queue.Contracts:ICommand', because exclude is true
        cfg.Publish<ICommand>(p => p.Exclude = true);
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
