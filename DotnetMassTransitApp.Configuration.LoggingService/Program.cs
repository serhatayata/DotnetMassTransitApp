using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

var environmentName = environment.EnvironmentName;

#region SERILOG

var serilogConfiguration = new ConfigurationBuilder()
    .AddJsonFile("Configurations/Settings/serilog.json",
                 optional: false,
                 reloadOnChange: true)
    .AddJsonFile($"Configurations/Settings/serilog.{environmentName}.json",
                 optional: true,
                 reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Async(writeTo => writeTo.Console(new Serilog.Formatting.Json.JsonFormatter()))
                .WriteTo.Async(writeTo => writeTo.Debug(new RenderedCompactJsonFormatter()))
                .WriteTo.Async(writeTo => writeTo.Elasticsearch(ConfigureElasticSink(serilogConfiguration, environmentName)))
                .Enrich.WithProperty("Environment", environmentName)
                .ReadFrom.Configuration(serilogConfiguration)
                .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
{
    var connString = configuration.GetSection("ElasticSearchLogOptions:ConnectionString").Value;

    return new ElasticsearchSinkOptions(new Uri(connString))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"{Assembly.GetExecutingAssembly()?.GetName()?.Name?.ToLowerInvariant().Replace(".", "-")}-{environment?.ToLowerInvariant().Replace(".", "-")}",
        TypeName = null,
        BatchAction = ElasticOpType.Create,
        CustomFormatter = new ElasticsearchJsonFormatter(),
        OverwriteTemplate = true,
        DetectElasticsearchVersion = true,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
        NumberOfReplicas = 1,
        NumberOfShards = 2,
        FailureCallback = (e,s) => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                           EmitEventFailureHandling.WriteToFailureSink |
                           EmitEventFailureHandling.RaiseCallback |
                           EmitEventFailureHandling.ThrowException
    };
}
