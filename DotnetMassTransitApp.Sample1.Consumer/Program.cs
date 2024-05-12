using DotnetMassTransitApp.Sample1.Consumer.Consumers;
using MassTransit;
using Shared.Queue.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(mt =>
{
    mt.AddConsumer<SubmitOrderConsumer>();

    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(host: queueSettings.Host);

        cfg.ReceiveEndpoint(queueName: "submit-order", ep =>
        {
            ep.ConfigureConsumer<SubmitOrderConsumer>(cntx);
            ep.Bind(exchangeName: "sample1-exchange-submit-order", opt =>
            {
                opt.RoutingKey = "sample1-routing-submit-order.#";
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
