using DotnetMassTransitApp.Sample1.Producer.Filters;
using MassTransit;
using MassTransit.Transports.Fabric;
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

        //Topic exchange
        cfg.Message<SubmitOrder>(x =>
        {
            x.SetEntityName("submit-order-exchange");
        });

        cfg.Publish<SubmitOrder>(opt =>
        {
            opt.ExchangeType = "topic";
            opt.AutoDelete = false;
            opt.Durable = true;

            //If we want to create queue and exchange here, then we use this
            //opt.BindQueue(exchangeName: "sample1-exchange-submit-order",
            //              queueName: "sample1-queue-submit-order",
            //              queueOpt =>
            //              {
            //                  queueOpt.ExchangeType = "topic";
            //                  queueOpt.RoutingKey = "sample1-routing-submit-order.#";
            //              });
        });

        // Fanout Exchange
        cfg.Message<RefundOrder>(x =>
        {
            x.SetEntityName("refund-order-exchange");
        });

        cfg.Publish<RefundOrder>(opt =>
        {
            opt.ExchangeType = "fanout";
            opt.AutoDelete = false;
            opt.Durable = true;
        });

        /////

        cfg.Message<StartDelivery>(x =>
        {
            x.SetEntityName("start-delivery-exchange");
        });

        cfg.Publish<StartDelivery>(opt =>
        {
            opt.ExchangeType = "fanout";
            opt.AutoDelete = false;
            opt.Durable = true;
        });

        //Header exchange
        cfg.Message<NotificationSms>(x =>
        {
            x.SetEntityName("notification-sms-exchange");
        });

        cfg.Publish<NotificationSms>(opt =>
        {
            opt.ExchangeType = "headers";
            opt.AutoDelete = false;
            opt.Durable = true;
        });

        cfg.ConfigureSend(opt =>
        {
            opt.UseFilter(new MySendFilter<StartDelivery>());
        });

        cfg.SendTopology.UseCorrelationId<SubmitOrder>(x => x.OrderId);

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
