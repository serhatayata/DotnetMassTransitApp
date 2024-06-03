using DotnetMassTransitApp.Patterns.Saga.Consumer.Consumers;
using MassTransit;
using Shared.Queue.Models;
using Shared.Queue.Saga;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(mt =>
{
    mt.AddConsumer<OrderSubmittedConsumer>();
    mt.AddConsumer<UpdateAccountHistoryConsumer>();

    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(host: queueSettings.Host);

        // Fanout exchange consumer

        cfg.ReceiveEndpoint(queueName: "order-submitted", ep =>
        {
            ep.ConfigureConsumer<OrderSubmittedConsumer>(cntx);

            ep.Bind(exchangeName: "order-submitted-exchange", clb =>
            {
                clb.ExchangeType = "fanout";
                clb.AutoDelete = false;
                clb.Durable = true;
            });
        });

        cfg.ReceiveEndpoint(queueName: "update-account-history", ep =>
        {
            ep.ConfigureConsumer<UpdateAccountHistoryConsumer>(cntx);

            ep.Bind(exchangeName: "update-account-history-exchange", clb =>
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
