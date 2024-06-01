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

        // Submit Order Fanout Exchange
        //cfg.Message<SubmitOrder>(x =>
        //{
        //    x.SetEntityName("submit-order-exchange");
        //});

        //cfg.Publish<SubmitOrder>(opt =>
        //{
        //    opt.ExchangeType = "fanout";
        //    opt.AutoDelete = false;
        //    opt.Durable = true;
        //});

        // Order accepted fanout exchange

        //cfg.Message<OrderAccepted>(x =>
        //{
        //    x.SetEntityName("order-accepted-exchange");
        //});

        //cfg.Publish<OrderAccepted>(opt =>
        //{
        //    opt.ExchangeType = "fanout";
        //    opt.AutoDelete = false;
        //    opt.Durable = true;
        //});

        cfg.SendTopology.UseCorrelationId<SubmitOrder>(x => x.OrderId);
        cfg.SendTopology.UseCorrelationId<OrderAccepted>(x => x.OrderId);

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
