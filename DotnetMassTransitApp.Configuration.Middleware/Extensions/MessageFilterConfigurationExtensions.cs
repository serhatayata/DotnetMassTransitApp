using DotnetMassTransitApp.Configuration.Middleware.ConfigurationObservers;
using MassTransit;

namespace DotnetMassTransitApp.Configuration.Middleware.Extensions;

public static class MessageFilterConfigurationExtensions
{
    public static void UseMessageFilter(this IConsumePipeConfigurator configurator)
    {
        if (configurator == null)
            throw new ArgumentNullException(nameof(configurator));

        var observer = new MessageFilterConfigurationObserver(configurator);
    }
}