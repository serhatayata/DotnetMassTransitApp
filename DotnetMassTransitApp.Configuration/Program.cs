using DotnetMassTransitApp.Configuration.Consumers;
using MassTransit;
using Shared.Queue.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(s => s.AddConsole().AddDebug());

builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options =>
    {
        // By default, MassTransit connects to the broker asynchronously. When set to true, the MassTransit Hosted Service
        // will block startup until the broker connection has been established.
        options.WaitUntilStarted = true;

        // By default, MassTransit waits infinitely until the broker connection is established. If specified, MassTransit
        // will give up after the timeout has expired.
        options.StartTimeout = TimeSpan.FromSeconds(20);

        // MassTransit waits infinitely for the bus to stop, including any active message consumers. If specified,
        // MassTransit will force the bus to stop after the timeout has expired.
        options.StopTimeout = TimeSpan.FromHours(1);

        // If specified, the ConsumeContext.CancellationToken will be canceled after the specified timeout when the bus
        // is stopping. This allows long-running consumers to observe the cancellation token and react accordingly. Must be
        // <= the StopTimeout
        options.ConsumerStopTimeout = TimeSpan.FromMinutes(30);
    });

builder.Services.AddMassTransit(mt =>
{
    mt.SetKebabCaseEndpointNameFormatter();

    mt.AddConsumer<SubmitOrderConsumer>().ExcludeFromConfigureEndpoints();

    mt.AddConfigureEndpointsCallback((name, cfg) =>
    {
        // To conditionally apply transport-specific settings, the cfg parameter can be pattern-matched to the transport type
        if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
            rmq.SetQuorumQueue(3);

        cfg.UseMessageRetry(r => r.Immediate(2));
    });

    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(host: queueSettings.Host);

        cfg.ReceiveEndpoint(queueName: "submit-order", ep =>
        {
            ep.ConfigureConsumer<SubmitOrderConsumer>(cntx);
        });

        //cfg.ConfigureEndpoints(cntx);
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
