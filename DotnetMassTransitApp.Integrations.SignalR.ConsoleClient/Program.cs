using DotnetMassTransitApp.Integrations.SignalR.ConsoleClient;

var builder = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true);
        config.AddEnvironmentVariables();

        if (args != null)
            config.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<ChatHubConnectionHostedService>();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });


await builder.RunConsoleAsync();
