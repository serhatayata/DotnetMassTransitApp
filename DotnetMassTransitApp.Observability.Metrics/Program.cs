using MassTransit.Monitoring;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(ConfigureResource)
    .WithMetrics(b => b
        .AddMeter(InstrumentationOptions.MeterName) // MassTransit Meter
        .AddConsoleExporter() // Any OTEL suportable exporter can be used here
    );

// WILL CONTINUE ...

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

void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}