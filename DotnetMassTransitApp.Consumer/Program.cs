using DotnetMassTransitApp.Consumer.Consumers;
using DotnetMassTransitApp.Consumer.Models;
using MassTransit;
using Shared.Queue.Contracts;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(s => s.AddConsole().AddDebug());

builder.Services.AddScoped<IOrderSubmitter, OrderSubmitter>();

builder.Services.AddMassTransit(mt =>
{
    mt.SetKebabCaseEndpointNameFormatter();

    mt.AddConsumer<SubmitOrderConsumer>();
    mt.AddConsumer<FinalizeOrderConsumer>();

    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();
        //cfg.Host(queueSettings.Uri, c =>
        //{
        //    c.Username(queueSettings.Username);
        //    c.Password(queueSettings.Password);
        //});

        cfg.Host(host: queueSettings.Host);

        EndpointConvention.Map<StartDelivery>(new Uri(configuration.GetSection("deliveryServiceQueue").Value));

        cfg.ReceiveEndpoint(queueName: "submit-order", ep =>
        {
            ep.ConfigureConsumer<SubmitOrderConsumer>(cntx);
        });

        cfg.ReceiveEndpoint(queueName: "finalize-order-request", ep =>
        {
            ep.ConfigureConsumer<FinalizeOrderConsumer>(cntx);
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
