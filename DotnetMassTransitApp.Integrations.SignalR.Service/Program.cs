using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.SignalR.Contracts;
using MassTransit.SignalR.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

IReadOnlyList<IHubProtocol> protocols = new IHubProtocol[] { new JsonHubProtocol() };
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host(new Uri("rabbitmq://localhost"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });
});

await busControl.StartAsync();

do
{
    Console.WriteLine("Enter hub message (or quit to exit)");
    Console.Write("> ");
    var value = Console.ReadLine();

    if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
        break;

    //An All<ChatHub> message, including your payload will be published to your configured broker.
    //All (or a select few, depending on how you configured MassTransit) consumers which registered.AddSignalRHub<ChatHub>(...) will receive the message.
    //The message will be published through the SignalR connection.
    //Your client will receive the message.
    await busControl.Publish<All<DotnetMassTransitApp.Integrations.SignalR.MVC.Hubs.ChatHub>>(new
    {
        Messages = protocols.ToProtocolDictionary("broadcastMessage", new object[] { "backend-process-hubprotocol", value })
    });

    //await busControl.Publish<Group<DotnetMassTransitApp.Integrations.SignalR.MVC.Hubs.ChatHub>>(new
    //{
    //    GroupName = "ServiceDeskEmployees",
    //    ExcludedConnectionIds = new[] { "11b9c749-69a2-4f3e-8a8b-968122156220", "1737778b-c836-4023-a255-51c2e4898c43" },
    //    Messages = protocols.ToProtocolDictionary("broadcastMessage", new object[] { "backend-process", "Hello" })
    //});
}
while (true);

await busControl.StopAsync();

namespace DotnetMassTransitApp.Integrations.SignalR.MVC.Hubs
{
    using Microsoft.AspNetCore.SignalR;

    public class ChatHub : Hub
    {
        // Actual implementation in the other project, but MT Needs the hub for the generic message type
    }
}