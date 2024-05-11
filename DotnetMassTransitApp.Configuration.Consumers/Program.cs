using DotnetMassTransitApp.Configuration.Consumers.Consumers;
using DotnetMassTransitApp.Configuration.Consumers.Definitions;
using MassTransit;
using Shared.Queue.Contracts;
using Shared.Queue.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

builder.Services.AddMassTransit(x =>
{
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "", includeNamespace: false));

    //x.AddConsumer<MyConsumer>();
    //x.AddConsumer(typeof(MyConsumer));

    //x.AddConsumer<MyConsumer, MyConsumerDefinition>();
    //x.AddConsumer(typeof(MyConsumer), typeof(MyConsumerDefinition));

    // Adds a consumer with a matching consumer definition and configures the consumer pipeline.
    x.AddConsumer<NotificationSmsConsumer, NotificationSmsConsumerDefinition>(cfg =>
    {
        cfg.ConcurrentMessageLimit = 8;
    });

    x.AddConsumer<ChangeLikeBatchConsumer>(cfg =>
    {
        cfg.Options<BatchOptions>(options =>
        {
            options.SetMessageLimit(100)
                   .SetTimeLimit(s: 1)
                   .SetTimeLimitStart(BatchTimeLimitStart.FromLast)
                   .GroupBy<ChangeLike, int>(x => x.Message.ProductId)
                   .SetConcurrencyLimit(10);
        });
    });

    x.SetJobConsumerOptions();

    x.UsingRabbitMq((cntx, cfg) =>
    {
        cfg.Host(host: queueSettings.Host);

        cfg.PrefetchCount = 32; // applies to all receive endpoints

        cfg.ReceiveEndpoint(queueName: "notification-sms", ep =>
        {
            ep.ConfigureConsumer<NotificationSmsConsumer>(cntx);
        });

        cfg.ReceiveEndpoint(queueName: "change-like", ep =>
        {
            ep.ConcurrentMessageLimit = 28; // only applies to this endpoint
            ep.ConfigureConsumer<ChangeLikeBatchConsumer>(cntx);
        });

        var serviceInstanceOptions = new ServiceInstanceOptions()
            .SetEndpointNameFormatter(cntx.GetService<IEndpointNameFormatter>() ?? KebabCaseEndpointNameFormatter.Instance);

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
