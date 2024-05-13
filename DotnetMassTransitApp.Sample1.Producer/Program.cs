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
