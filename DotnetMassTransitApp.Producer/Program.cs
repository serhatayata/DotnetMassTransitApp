using MassTransit;
using Shared.Queue.Contracts;
using Shared.Queue.Events;
using Shared.Queue.Models;
using Shared.Queue.Requests;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<QueueSettings>("QueueSettings", configuration);

builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumers(typeof(Program).Assembly);
    cfg.AddRequestClient(typeof(CheckOrderStatus));
});

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

        EndpointConvention.Map<SubmitOrder>(new Uri("rabbitmq://localhost/submit-order"));
        EndpointConvention.Map<SendNotificationOrder>(new Uri("rabbitmq://localhost/send-notification-order"));
        EndpointConvention.Map<StartDelivery>(new Uri("rabbitmq://localhost/start-delivery"));
        EndpointConvention.Map<FinalizeOrderRequest>(new Uri("rabbitmq://localhost/finalize-order-request"));
        EndpointConvention.Map<CheckOrderStatus>(new Uri("rabbitmq://localhost/check-order-status"));
        EndpointConvention.Map<CancelOrder>(new Uri("rabbitmq://localhost/cancel-order"));
        EndpointConvention.Map<RefundOrder>(new Uri("rabbitmq://localhost/refund-order"));
        EndpointConvention.Map<NotificationSms>(new Uri("rabbitmq://localhost/notification-sms"));
        EndpointConvention.Map<ChangeLike>(new Uri("rabbitmq://localhost/change-like"));

        cfg.SendTopology.UseCorrelationId<NotificationSms>(x => x.OrderId);
        cfg.SendTopology.UseCorrelationId<SubmitOrder>(x => x.OrderId);
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
