using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.Configuration;
using Shared.Queue.Contracts;
using Shared.Queue.Events;
using Shared.Queue.Saga;
using Shared.Queue.Saga.Events;
using Shared.Queue.Requests;
using System.Threading.Channels;
using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Reflection;
using Shared.Queue.Saga.Contracts;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine;

public class OrderStateMachine :
    MassTransitStateMachine<OrderState>
{
    public Event<SubmitOrder> SubmitOrder { get; private set; }
    public Event<OrderAccepted> OrderAccepted { get; private set; }
    public Event<ExternalOrderSubmitted> ExternalOrderSubmitted { get; private set; }
    public Event<RequestOrderCancellation> OrderCancellationRequested { get; private set; }
    public Event<OrderCompleted> OrderCompleted { get; private set; }
    public Event<CreateOrder> OrderSubmitted { get; set; }


    // Added for composite event
    public Event OrderReady { get; private set; }

    public Request<OrderState, ProcessOrder, OrderProcessed> ProcessOrder { get; set; }

    public Schedule<OrderState, OrderCompletionTimeoutExpired> OrderCompletionTimeout { get; private set; }

    public State Submitted { get; private set; }
    public State Accepted { get; private set; }
    public State Completed { get; private set; }
    public State Canceled { get; private set; }
    public State Created { get; set; }


    public OrderStateMachine()
    {
        //////////////////// PART 1 ////////////////////

        /// the Initially block is used to define the behavior of the SubmitOrder event during the Initial state. 
        /// When a SubmitOrder message is consumed and an instance with a CorrelationId matching the OrderId is not found, 
        /// a new instance will be created in the Initial state. The TransitionTo activity transitions the instance to 
        /// the Submitted state, after which the instance is persisted using the saga repository.

        Initially(
            When(SubmitOrder)
                .TransitionTo(Submitted));

        // Subsequently, the OrderAccepted event could be handled by the behavior shown below.

        // This can also be used to specify Order id as correlation id
        //GlobalTopology.Send.UseCorrelationId<SubmitOrder>(x => x.OrderId);

        Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.OrderId));

        During(Submitted,
            When(OrderAccepted)
                .TransitionTo(Accepted));



        //////////////////// PART 2 ////////////////////

        // Receiving a SubmitOrder message after an OrderAccepted event could cause the SubmitOrder message to end up in
        // the _error queue. If the OrderAccepted event is received first, it would be discarded since it isn't accepted
        // in the Initial state. Below is an updated state machine that handles both of these scenarios.

        //Initially(
        //    When(SubmitOrder)
        //        .TransitionTo(Submitted),
        //    When(OrderAccepted)
        //        .TransitionTo(Accepted));

        //During(Submitted,
        //    When(OrderAccepted)
        //        .TransitionTo(Accepted));

        //During(Accepted,
        //    Ignore(SubmitOrder));



        //////////////////// PART 3 ////////////////////

        // Receiving a SubmitOrder message while in an Accepted state ignores the event. However, data in the event may
        // be useful. In that case, adding behavior to copy the data to the instance could be added. Below, data from
        // the event is captured in both scenarios.

        //Initially(
        //    When(SubmitOrder)
        //        .Then(x => x.Saga.OrderDate = x.Message.OrderDate)
        //        .TransitionTo(Submitted),
        //    When(OrderAccepted)
        //        .TransitionTo(Accepted));

        //During(Submitted,
        //    When(OrderAccepted)
        //        .TransitionTo(Accepted));

        //During(Accepted,
        //    When(SubmitOrder)
        //        .Then(x => x.Saga.OrderDate = x.Message.OrderDate));





        //When the event doesn't have a Guid that uniquely correlates to an instance, the .SelectId expression must be configured. In the below example, NewId is used to generate a sequential identifier which will be assigned to the instance CorrelationId. Any property on the event can be used to initialize the CorrelationId.

        //Event(() => ExternalOrderSubmitted, e => e
        //    .CorrelateBy(i => i.OrderNumber, x => x.Message.OrderNumber)
        //    .SelectId(x => NewId.NextGuid()));

        //Event(() => ExternalOrderSubmitted, e => e
        //    .CorrelateBy((instance, context) => instance.OrderNumber == context.Message.OrderNumber)
        //    .SelectId(x => NewId.NextGuid()));





        // COMPOSITE EVENT

        // A composite event is configured by specifying one or more events that must be consumed, after which the composite event will be raised.A composite event uses an instance property to keep track of the required events, which is   specified during configuration.
        // To define a composite event, the required events must first be configured along with any event behaviors, after   which the composite event can be configured.

        //Initially(
        //    When(SubmitOrder)
        //        .TransitionTo(Submitted),
        //    When(OrderAccepted)
        //        .TransitionTo(Accepted));

        //During(Submitted,
        //    When(OrderAccepted)
        //        .TransitionTo(Accepted));

        // NOTE : The order of events being declared can impact the order in which they execute. Therefore, it is best to           declare composite events at the end of the state machine declaration, after all other events and behaviors        are declared. That way, the composite events will be raised after the dependent event behaviors.

        // Once the SubmitOrder and OrderAccepted events have been consumed, the OrderReady event will be triggered.
        //CompositeEvent(() => OrderReady, x => x.ReadyEventStatus, SubmitOrder, OrderAccepted);

        //DuringAny(
        //    When(OrderReady)
        //        .Then(context => Console.WriteLine("Order Ready: {0}", context.Saga.CorrelationId)));





        // MISSING INSTANCE

        //In this example, when a cancel order request is consumed without a matching instance, a response will be sent that
        //the order was not found.Instead of generating a Fault, the response is more explicit. Other missing
        //instance options include Discard, Fault, and Execute (a synchronous version of ExecuteAsync).

        //Event(() => OrderCancellationRequested, e =>
        //{
        //    e.CorrelateById(context => context.Message.OrderId);

        //    e.OnMissingInstance(m =>
        //    {
        //        return m.ExecuteAsync(x => x.RespondAsync<OrderNotFound>(new OrderNotFound() { OrderId = x.Message.OrderId}));
        //    });
        //});




        // INITIAL INSERT

        //To increase new instance performance, configuring an event to directly insert into a saga repository may reduce lock contention.To configure an event to insert, it should be in the Initially block, as well as have a saga factory specified.

        //When using InsertOnInitial, it is critical that the saga repository is able to detect duplicate keys(in this case, CorrelationId - which is initialized using OrderId). In this case, having a clustered primary key on CorrelationId would prevent duplicate instances from being inserted. If an event is correlated using a different property, make sure that the database enforces a unique constraint on the instance property and the saga factory initializes the instance property with the event property value.

        //Event(() => SubmitOrder, e =>
        //{
        //    e.CorrelateById(context => context.Message.OrderId);

        //    e.InsertOnInitial = true;
        //    e.SetSagaFactory(context => new OrderState
        //    {
        //        CorrelationId = context.Message.OrderId
        //    });
        //});

        //Initially(
        //    When(SubmitOrder)
        //        .TransitionTo(Submitted));

        //The database would use a unique constraint on the OrderNumber to prevent duplicates, which the saga repository would detect as an existing instance, which would then be loaded to consume the event.

        //Event(() => ExternalOrderSubmitted, e =>
        //{
        //    e.CorrelateBy(i => i.OrderNumber, x => x.Message.OrderNumber);
        //    e.SelectId(x => NewId.NextGuid());

        //    e.InsertOnInitial = true;
        //    e.SetSagaFactory(context => new OrderState
        //    {
        //        CorrelationId = context.CorrelationId ?? NewId.NextGuid(),
        //        OrderNumber = context.Message.OrderNumber,
        //    });
        //});

        //Initially(
        //    When(SubmitOrder)
        //        .TransitionTo(Submitted));




        // COMPLETED INSTANCE

        // By default, instances are not removed from the saga repository. To configure completed instance removal, specify  the method used to determine if an instance has completed.

        //Event(() => OrderCompleted, x => x.CorrelateById(context => context.Message.OrderId));

        //DuringAny(
        //    When(OrderCompleted)
        //        .Finalize());

        //SetCompletedWhenFinalized();

        //When the instance consumes the OrderCompleted event, the instance is finalized (which transitions the instance
        //to the Final state). The SetCompletedWhenFinalized method defines an instance in the Final state as completed
        //– which is then used by the saga repository to remove the instance.

        //To use a different completed expression, such as one that checks if the instance is in a Completed state, use the SetCompleted method

        //Event(() => OrderCompleted, x => x.CorrelateById(context => context.Message.OrderId));

        //DuringAny(
        //    When(OrderCompleted)
        //        .TransitionTo(Completed));

        //SetCompleted(async instance =>
        //{
        //    State<TInstance> currentState = await this.GetState(instance);

        //    return Completed.Equals(currentState);
        //});




        // ACTIVITIES



        // PUBLISH

        //Initially(
        //    When(SubmitOrder)
        //        .Publish(context => 
        //            new OrderSubmitted() { OrderId = context.Saga.CorrelationId, OrderDate = context.Saga.OrderDate})
        //        .TransitionTo(Submitted));

        // Alternatively, a message initializer can be used to eliminate the Event class.

        //Initially(
        //    When(SubmitOrder)
        //        .PublishAsync(context => context.Init<OrderSubmitted>(
        //            new OrderSubmitted { OrderId = context.Saga.CorrelationId, OrderDate = context.Saga.OrderDate }))
        //        .TransitionTo(Submitted));




        // SEND

        //var accountServiceAddress = new Uri(string.Empty); // can be set
        //Initially(
        //    When(SubmitOrder)
        //        .Send(accountServiceAddress, 
        //              context => new UpdateAccountHistory() { OrderId = context.Saga.CorrelationId })
        //        .TransitionTo(Submitted));

        //// Alternatively

        //Initially(
        //    When(SubmitOrder)
        //        .SendAsync(accountServiceAddress, 
        //                   context => context.Init<UpdateAccountHistory>(new { OrderId = context.Saga.CorrelationId }))
        //        .TransitionTo(Submitted));




        // RESPOND

        //A state machine can respond to requests by configuring the request message type as an event, and using the Respond
        //method.When configuring a request event, configuring a missing instance method is recommended, to provide a better
        //response experience (either through a different response type, or a response that indicates an instance was not
        //found).

        //Event(() => OrderCancellationRequested, e =>
        //{
        //    e.CorrelateById(context => context.Message.OrderId);

        //    e.OnMissingInstance(m =>
        //    {
        //        return m.ExecuteAsync(x => x.RespondAsync<OrderNotFound>(new OrderNotFound() { OrderId = x.Message.OrderId }));
        //    });
        //});

        //DuringAny(
        //    When(OrderCancellationRequested)
        //        .RespondAsync(context => context.Init<OrderCanceled>(new { OrderId = context.Saga.CorrelationId }))
        //        .TransitionTo(Canceled));


        // There are scenarios where it is required to wait for the response from the state machine. In these scenarios
        // the information that is required to respond to the original request should be stored.

        //InstanceState(m => m.CurrentState);
        //Event(() => OrderSubmitted);
        //Request(() => ProcessOrder, order => order.ProcessingId, config => { config.Timeout = TimeSpan.Zero; });

        //Initially(
        //    When(OrderSubmitted)
        //        .Then(context =>
        //        {
        //            context.Saga.CorrelationId = context.Message.CorrelationId;
        //            context.Saga.ProcessingId = Guid.NewGuid();

        //            context.Saga.OrderId = Guid.NewGuid();

        //            context.Saga.RequestId = context.RequestId;
        //            context.Saga.ResponseAddress = context.ResponseAddress;
        //        })
        //        .Request(ProcessOrder, context => new ProcessOrder() { OrderId = context.Saga.OrderId, ProcessingId = context.Saga.ProcessingId!.Value })
        //        .TransitionTo(ProcessOrder.Pending));

        //During(ProcessOrder.Pending,
        //    When(ProcessOrder.Completed)
        //        .TransitionTo(Created)
        //        .ThenAsync(async context =>
        //        {
        //            var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
        //            await endpoint.Send(context.Saga, r => r.RequestId = context.Saga.RequestId);
        //        }),
        //    When(ProcessOrder.Faulted)
        //        .TransitionTo(Canceled)
        //        .ThenAsync(async context =>
        //        {
        //            var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
        //            await endpoint.Send(new OrderCanceled() { OrderId = context.Saga.OrderId, Reason = "Faulted" }, r => r.RequestId = context.Saga.RequestId);
        //        }),
        //    When(ProcessOrder.TimeoutExpired)
        //        .TransitionTo(Canceled)
        //        .ThenAsync(async context =>
        //        {
        //            var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
        //            await endpoint.Send(new OrderCanceled() { OrderId = context.Saga.OrderId, Reason = "Timeout"}, r => r.RequestId = context.Saga.RequestId);
        //        }));




        // Schedule

        //A state machine can schedule events, which uses the message scheduler to schedule a message for delivery to the instance.First, the schedule must be declared.

        // The configuration specifies the Delay, which can be overridden by the schedule activity, and the correlation      expression for the Received event. The state machine can consume the Received event as shown. The                 OrderCompletionTimeoutTokenId is a Guid? instance property used to keep track of the scheduled message tokenId    which can later be used to unschedule the event.

        //Schedule(() => OrderCompletionTimeout, instance => instance.OrderCompletionTimeoutTokenId, s =>
        //{
        //    s.Delay = TimeSpan.FromDays(30);

        //    s.Received = r => r.CorrelateById(context => context.Message.OrderId);
        //});

        //During(Accepted,
        //    When(OrderCompletionTimeout.Received)
        //        .PublishAsync(context => context.Init<OrderCompleted>(new { OrderId = context.Saga.CorrelationId }))
        //        .Finalize());

        // As stated below, the delay can be overridden by the Schedule activity. Both instance and message (context.Data) content can be used to calculate the delay.

        //During(Submitted,
        //    When(OrderAccepted)
        //        .Schedule(OrderCompletionTimeout, context => context.Init<OrderCompletionTimeoutExpired>(new { OrderId = context.Saga.CorrelationId }))
        //        .TransitionTo(Accepted));

        //During(Submitted,
        //    When(OrderAccepted)
        //        .Schedule(OrderCompletionTimeout, context => context.Init<OrderCompletionTimeoutExpired>(new { OrderId = context.Saga.CorrelationId }),
        //            context => context.Message.CompletionTime)
        //        .TransitionTo(Accepted));

        // Once the scheduled event is received, the OrderCompletionTimeoutTokenId property is cleared.
        // If the scheduled event is no longer needed, the Unschedule activity can be used.

        //DuringAny(
        //    When(OrderCancellationRequested)
        //        .RespondAsync(context => context.Init<OrderCanceled>(new { OrderId = context.Saga.CorrelationId }))
        //        .Unschedule(OrderCompletionTimeout)
        //        .TransitionTo(Canceled));




        // Request

        Continues
    }

}