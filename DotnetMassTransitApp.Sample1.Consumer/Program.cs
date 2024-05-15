using DotnetMassTransitApp.Sample1.Consumer.Consumers;
using MassTransit;
using Shared.Queue.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(mt =>
{
    mt.AddConsumer<SubmitOrderConsumer>();
    mt.AddConsumer<ChangeLikeConsumer>();
    mt.AddConsumer<RefundOrderConsumer>();

    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(host: queueSettings.Host);

        // Topic exchange consumer
        cfg.ReceiveEndpoint(queueName: "submit-order", ep =>
        {
            ep.Bind(exchangeName: "submit-order-exchange", opt =>
            {
                opt.RoutingKey = "sample1-routing-submit-order.#";
                opt.ExchangeType = "topic";
                opt.AutoDelete = false;
                opt.Durable = true;
            });

            ep.ConfigureConsumer<SubmitOrderConsumer>(cntx);
        });

        // Direct exchange consumer
        cfg.ReceiveEndpoint(queueName: "change-like", ep =>
        {
            ep.ExchangeType = "direct";
            ep.Bind(exchangeName: "change-like-exchange", opt =>
            {
                opt.ExchangeType = "direct";
                opt.AutoDelete = false;
                opt.Durable = true;
            });

            ep.ConfigureConsumer<ChangeLikeConsumer>(cntx);
        });

        // Fanout exchange consumer

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
