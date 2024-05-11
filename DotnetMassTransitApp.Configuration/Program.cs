using DotnetMassTransitApp.Configuration.Consumers;
using DotnetMassTransitApp.Configuration.Definitions;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Queue.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(s => s.AddConsole().AddDebug());

var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

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

#region HealthCheck
builder.Services.AddHealthChecks()
                .AddRabbitMQ(rabbitConnectionString: queueSettings.Host, tags: new List<string>() { "ready" });
#endregion

#region MassTransit configuration
builder.Services.AddMassTransit(mt =>
{
    mt.ConfigureHealthCheckOptions(options =>
    {
        options.Name = "masstransit";
        options.MinimalFailureStatus = HealthStatus.Unhealthy;
        options.Tags.Add("health");
    });

    mt.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "", includeNamespace: false));

    mt.AddConsumer<SubmitOrderConsumer, SubmitOrderConsumerDefinition>()
      .Endpoint(e =>
      {
          //If we don't use ReceiveEndpoint then this will be used

          // override the default endpoint name
          e.Name = "order-service-extreme";

          // specify the endpoint as temporary (may be non-durable, auto-delete, etc.)
          e.Temporary = false;

          // specify an optional concurrent message limit for the consumer
          e.ConcurrentMessageLimit = 8;

          // only use if needed, a sensible default is provided, and a reasonable
          // value is automatically calculated based upon ConcurrentMessageLimit if
          // the transport supports it.
          e.PrefetchCount = 16;

          // set if each service instance should have its own endpoint for the consumer
          // so that messages fan out to each instance.
          e.InstanceId = "something-unique";

          // If you want to prevent the consumer from creating topics/exchanges
          // for consumed message types when started.
          e.ConfigureConsumeTopology = false;
      });
    //.ExcludeFromConfigureEndpoints();

    //mt.AddConfigureEndpointsCallback((name, cfg) =>
    //{
    //    // To conditionally apply transport-specific settings, the cfg parameter can be pattern-matched to the transport type
    //    if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
    //        rmq.SetQuorumQueue(3);

    //    cfg.UseMessageRetry(r => r.Immediate(2));
    //});

    mt.UsingRabbitMq((cntx, cfg) =>
    {
        cfg.Host(host: queueSettings.Host);

        cfg.PrefetchCount = 32; // applies to all receive endpoints

        cfg.ReceiveEndpoint(queueName: "submit-order", ep =>
        {
            ep.ConcurrentMessageLimit = 28; // only applies to this endpoint
            ep.ConfigureConsumer<SubmitOrderConsumer>(cntx);

            // When using ConfigureConsumer with a consumer that has a definition, the EndpointName, PrefetchCount,
            // and Temporary properties of the consumer definition are not used.
        });

        //cfg.ReceiveEndpoint(new TemporaryEndpointDefinition(), ep =>
        //{
        //    ep.ConfigureConsumer<SubmitOrderConsumer>(cntx);
        //});

        cfg.ConfigureEndpoints(cntx);
    });
});
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapHealthChecks("/health/ready", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("health"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
