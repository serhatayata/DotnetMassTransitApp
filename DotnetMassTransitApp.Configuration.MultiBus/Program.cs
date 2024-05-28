using DotnetMassTransitApp.Configuration.MultiBus.Consumers;
using MassTransit;
using Shared.Queue.Buses;
using Shared.Queue.Contracts;
using Shared.Queue.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// BUS 1
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();

    x.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(queueSettings.Host);

        cfg.ReceiveEndpoint(queueName: "submit-order", ep =>
        {
            ep.ConfigureConsumer<SubmitOrderConsumer>(cntx);

            ep.Bind(exchangeName: "submit-order-exchange", clb =>
            {
                clb.ExchangeType = "fanout";
                clb.AutoDelete = false;
                clb.Durable = true;
            });
        });

        cfg.ConfigureEndpoints(cntx);
    });
});

// BUS 2

/// The generic argument, ISecondBus, is the type that will be added to the container instead of IBus. This ensures 
/// that access to the additional bus is directly available without confusion.
builder.Services.AddMassTransit<ISecondBus>(x =>
{
    x.AddConsumer<RefundOrderConsumer>();

    x.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(queueSettings.Host);

        cfg.ReceiveEndpoint(queueName: "refund-order", ep =>
        {
            ep.ConfigureConsumer<RefundOrderConsumer>(cntx);

            ep.Bind(exchangeName: "refund-order-exchange", clb =>
            {
                clb.ExchangeType = "fanout";
                clb.AutoDelete = false;
                clb.Durable = true;
            });
        });

        cfg.ConfigureEndpoints(cntx);
    });
});


// Third bus
builder.Services.AddMassTransit<IThirdBus>(x =>
{
    x.AddConsumer<SendNotificationOrderConsumer>();

    x.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(queueSettings.Host);

        cfg.ReceiveEndpoint(queueName: "send-notification-order", ep =>
        {
            ep.ConfigureConsumer<SendNotificationOrderConsumer>(cntx);

            ep.Bind(exchangeName: "send-notification-order-exchange", clb =>
            {
                clb.ExchangeType = "fanout";
                clb.AutoDelete = false;
                clb.Durable = true;
            });
        });

        cfg.ConfigureEndpoints(cntx);
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
