using DotnetMassTransitApp.Configuration.Middleware.Consumers;
using DotnetMassTransitApp.Configuration.Middleware.Extensions;
using MassTransit;
using Shared.Queue.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(mt =>
{
    mt.AddConsumer<RefundOrderConsumer>();

    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(host: queueSettings.Host);

        // Fanout exchange consumer

        cfg.ReceiveEndpoint(queueName: "refund-order", ep =>
        {
            ep.ConfigureConsumer<RefundOrderConsumer>(cntx);
            ep.UseMessageFilter();
            ep.Bind(exchangeName: "refund-order-exchange", clb =>
            {
                clb.ExchangeType = "fanout";
                clb.AutoDelete = false;
                clb.Durable = true;
            });
        });

        cfg.UseRateLimit(1000, TimeSpan.FromSeconds(5));

        cfg.UseConcurrencyLimit(4);

        // In the above example, the kill switch will activate after 10 messages have been consumed. If the ratio of failures/    attempts exceeds 15%, the kill switch will trip and stop the receive endpoint. After 1 minute, the receive endpoint    will be restarted. Once restarted, if exceptions are still observed, the receive endpoint will be stopped again for 1  minute.
        cfg.UseKillSwitch(options => options
           .SetActivationThreshold(10)
           .SetTripThreshold(0.15)
           .SetRestartTimeout(m: 1));

        cfg.UseCircuitBreaker(cb =>
        {
            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
            cb.TripThreshold = 15;
            cb.ActiveThreshold = 10;
            cb.ResetInterval = TimeSpan.FromMinutes(5);
        });

        cfg.UseExceptionLogger();

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
