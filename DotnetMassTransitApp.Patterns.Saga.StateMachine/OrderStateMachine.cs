using MassTransit;
using Shared.Queue.Contracts;
using Shared.Queue.Saga;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine;

public class OrderStateMachine :
    MassTransitStateMachine<OrderState>
{
    public Event<SubmitOrder> SubmitOrder { get; private set; }
    public Event<OrderAccepted> OrderAccepted { get; private set; }

    public State Submitted { get; private set; }
    public State Accepted { get; private set; }

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

        Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.OrderId));

        During(Submitted,
            When(OrderAccepted)
                .TransitionTo(Accepted));

        //////////////////// PART 2 ////////////////////

        // Receiving a SubmitOrder message after an OrderAccepted event could cause the SubmitOrder message to end up in
        // the _error queue. If the OrderAccepted event is received first, it would be discarded since it isn't accepted
        // in the Initial state. Below is an updated state machine that handles both of these scenarios.

        Initially(
            When(SubmitOrder)
                .TransitionTo(Submitted),
            When(OrderAccepted)
                .TransitionTo(Accepted));

        During(Submitted,
            When(OrderAccepted)
                .TransitionTo(Accepted));

        During(Accepted,
            Ignore(SubmitOrder));

        //////////////////// PART 3 ////////////////////

        // Receiving a SubmitOrder message while in an Accepted state ignores the event. However, data in the event may
        // be useful. In that case, adding behavior to copy the data to the instance could be added. Below, data from
        // the event is captured in both scenarios.


    }

}