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
    mt.AddConsumer<SendNotificationOrderFaultConsumer>();

    mt.UsingRabbitMq((cntx, cfg) =>
    {
        var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();

        cfg.Host(host: queueSettings.Host);

        // if the initial 5 immediate retries fail (the database is really, really down), the message will retry an
        // additional three times after 5, 15, and 30 minutes. This could mean a total of 15 retry attempts (on top of
        // the initial 4 attempts prior to the retry/redelivery filters taking control).
        //cfg.UseDelayedRedelivery(r =>
        //{
        //    r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30));
        //});

        //cfg.UseMessageRetry(r => r.Immediate(5));

        cfg.ReceiveEndpoint(queueName: "send-notification-order", ep =>
        {
            ep.ConfigureConsumer<SendNotificationOrderConsumer>(cntx, c =>
            {
                //c.UseDelayedRedelivery(r =>
                //{
                //    r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30));
                //});

                c.UseMessageRetry(r =>
                {
                    r.Interval(1, TimeSpan.FromSeconds(1));
                    r.Ignore<ArgumentNullException>();
                    r.Ignore<DataException>(x => x.Message.Contains("SQL"));
                });
            });

            // global message retry settings for this queue
            //ep.UseMessageRetry(r =>
            //{
            //    r.Immediate(3);
            //    r.Handle<NotImplementedException>();
            //    r.Handle<ArgumentNullException>();
            //    r.Ignore(typeof(InvalidOperationException), typeof(InvalidCastException));
            //    r.Ignore<ArgumentException>(t => t.ParamName == "orderTotal");
            //});
        });

        cfg.UseInMemoryOutbox(cntx);

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
