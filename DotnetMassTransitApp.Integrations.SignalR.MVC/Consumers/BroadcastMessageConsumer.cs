﻿using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Shared.Queue.Contracts;
using Shared.Queue.Hubs;

namespace DotnetMassTransitApp.Configuration.Integrations.SignalR.MVC.Consumers;

public class BroadcastMessageConsumerDefinition : ConsumerDefinition<BroadcastMessageConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<BroadcastMessageConsumer> consumerConfigurator)
    {
        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator e)
        {
            e.AutoDelete = true;
            e.QueueExpiration = TimeSpan.FromSeconds(30);
        }
    }
}

public class BroadcastMessageConsumer : IConsumer<BroadcastMessage>
{
    private readonly IHubContext<ChatHub> _hubContext;

    public BroadcastMessageConsumer(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<BroadcastMessage> context)
    {
        await _hubContext.Clients.All.SendAsync("broadcastMessage", context.Message.Name, context.Message.Message);
    }
}