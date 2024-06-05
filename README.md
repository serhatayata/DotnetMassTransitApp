### MassTransit document implementation apps

- https://masstransit.io/documentation

```json
{

  "messageId": "50460000-adda-54e1-1fa3-08dc67547d10",

  "requestId": null,

  "correlationId": "7569172a-7c0e-44fb-a297-54bdebe6ce44",

  "conversationId": "50460000-adda-54e1-c760-08dc67547d15",

  "initiatorId": null,

  "sourceAddress": "rabbitmq://localhost/DESKTOPTNOQ92E_DotnetMassTran_bus_kbdyyyfp5jkqd4a4bdqgqidibq?temporary=true",

  "destinationAddress": "rabbitmq://localhost/submit-order",

  "responseAddress": null,

  "faultAddress": null,

  "messageType": [

    "urn:message:DotnetMassTransitApp.Producer.Contracts:SubmitOrder"

  ],

  "message": {

    "orderId": "7569172a-7c0e-44fb-a297-54bdebe6ce44"

  },

  "expirationTime": null,

  "sentTime": "2024-04-28T07:26:15.2890275Z",

  "headers": {},

  "host": {

    "machineName": "DESKTOP-******",

    "processName": "DotnetMassTransitApp.Producer",

    "processId": 18000,

    "assembly": "DotnetMassTransitApp.Producer",

    "assemblyVersion": "1.0.0.0",

    "frameworkVersion": "8.0.0",

    "massTransitVersion": "8.2.1.0",

    "operatingSystemVersion": "Microsoft Windows NT 10.0.19045.0"

  }

}
```

```txt
Above, we can see that "messageType" is "DotnetMassTransitApp.Producer.Contracts:SubmitOrder", 

when I create 2 applications for consumer and producer, both have different namespaces and so different messageTypes
when you send or publish a message from producer it cannot be consumed from consumer application because their messageTypes are different.

When this scenario happens, these messages would go to "_skipped" queue.
```

```txt
For "send-notification-order" queue,
When I send a message to this queue and consumer get the message, there might be an error. When this consumer throws an exception, first, this message goes to queue "send-notification-order-fault" and this message is consumed by "SendNotificationOrderFaultConsumer", after this consumer process, this message goes to "send-notification-order_error"

If we don't want this message to go to "send-notification-order_error", we use this below;
```

```csharp
cfg.ReceiveEndpoint(queueName: "send-notification-order", ep =>
{
   ep.DiscardFaultedMessages();
   ...
```
