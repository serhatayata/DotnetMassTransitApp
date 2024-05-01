using DotnetMassTransitApp.Exception.Consumers;
using DotnetMassTransitApp.Exception.Models;
using MassTransit;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(mt =>
{
    mt.SetKebabCaseEndpointNameFormatter();

    mt.AddConsumer<SendNotificationOrderConsumer>();

    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(host: queueSettings.Host);

        //cfg.UseMessageRetry(r => r.Immediate(5));

        cfg.ReceiveEndpoint(queueName: "send-notification-order", ep =>
        {
            ep.ConfigureConsumer<SendNotificationOrderConsumer>(cntx, c => c.UseMessageRetry(r =>
            {
                r.Interval(1, TimeSpan.FromSeconds(1));
                r.Ignore<ArgumentNullException>();
                r.Ignore<DataException>(x => x.Message.Contains("SQL"));
            }));

            // global message retry settings for this queue
            ep.UseMessageRetry(r =>
            {
                r.Immediate(3);
                r.Handle<NotImplementedException>();
                r.Handle<ArgumentNullException>();
                r.Ignore(typeof(InvalidOperationException), typeof(InvalidCastException));
                r.Ignore<ArgumentException>(t => t.ParamName == "orderTotal");
            });
        });
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
