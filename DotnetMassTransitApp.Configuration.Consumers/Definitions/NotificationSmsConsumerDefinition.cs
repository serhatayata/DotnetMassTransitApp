using DotnetMassTransitApp.Configuration.Consumers.Consumers;
using MassTransit;

namespace DotnetMassTransitApp.Configuration.Consumers.Definitions;

public class NotificationSmsConsumerDefinition : ConsumerDefinition<NotificationSmsConsumer>
{
    public NotificationSmsConsumerDefinition()
    {
        // override the default endpoint name, for whatever reason
        EndpointName = "ha-notification-sms";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 4;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator, 
        IConsumerConfigurator<NotificationSmsConsumer> consumerConfigurator, 
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}
