using DotnetMassTransitApp.Commands;
using DotnetMassTransitApp.Models;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<QueueSettings>("QueueSettings", configuration);

builder.Services.AddMassTransit(mt => 
              mt.AddMassTransit(mts => 
                 mts.UsingRabbitMq((cntx, cfg) =>
                 {
                     var queueSettings = configuration.GetSection("QueueSettings").Get<QueueSettings>();
                     cfg.Host(queueSettings.Uri, c =>
                     {
                         c.Username(queueSettings.Username);
                         c.Password(queueSettings.Password);
                     });

                     cfg.SendTopology.UseCorrelationId<SubmitOrder>(x => x.OrderId);
                 })));

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
