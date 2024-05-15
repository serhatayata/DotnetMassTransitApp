using MassTransit.Configuration;
using MassTransit;
using DotnetMassTransitApp.Configuration.Middleware.Specifications;

namespace DotnetMassTransitApp.Configuration.Middleware.ConfigurationObservers;

public class MessageFilterConfigurationObserver :
    ConfigurationObserver,
    IMessageConfigurationObserver
{
    public MessageFilterConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator)
        : base(receiveEndpointConfigurator)
    {
        Connect(this);
    }

    public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
        where TMessage : class
    {
        var specification = new MessageFilterPipeSpecification<TMessage>();

        configurator.AddPipeSpecification(specification);
    }
}