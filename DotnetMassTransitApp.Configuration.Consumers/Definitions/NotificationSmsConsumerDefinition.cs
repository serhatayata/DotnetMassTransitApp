using DotnetMassTransitApp.Configuration.Consumers.Consumers;
using MassTransit;

namespace DotnetMassTransitApp.Configuration.Consumers.Definitions;

public class NotificationSmsConsumerDefinition : ConsumerDefinition<NotificationSmsConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator, 
        IConsumerConfigurator<NotificationSmsConsumer> consumerConfigurator, 
        IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
    }
}
