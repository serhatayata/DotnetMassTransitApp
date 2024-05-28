using MassTransit;
using Shared.Queue.Buses;
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

        cfg.Host(queueSettings.Host);

        cfg.Message<SubmitOrder>(x =>
        {
            x.SetEntityName("submit-order-exchange");
        });

        cfg.Publish<SubmitOrder>(opt =>
        {
            opt.ExchangeType = "fanout";
            opt.AutoDelete = false;
            opt.Durable = true;
        });

        cfg.ConfigureEndpoints(cntx);
    });
});

builder.Services.AddMassTransit<ISecondBus>(mt =>
{
    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(queueSettings.Host);

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

        cfg.ConfigureEndpoints(cntx);
    });
});

builder.Services.AddMassTransit<IThirdBus>(mt =>
{
    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(queueSettings.Host);

        cfg.Message<SendNotificationOrder>(x =>
        {
            x.SetEntityName("send-notification-order-exchange");
        });

        cfg.Publish<SendNotificationOrder>(opt =>
        {
            opt.ExchangeType = "fanout";
            opt.AutoDelete = false;
            opt.Durable = true;
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
