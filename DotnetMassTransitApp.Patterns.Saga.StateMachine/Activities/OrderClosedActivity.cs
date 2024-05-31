using DotnetMassTransitApp.Patterns.Saga.StateMachine.Services;
using MassTransit;
using Shared.Queue.Events;
using Shared.Queue.Saga;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Activities;

public class OrderClosedActivity
    : IStateMachineActivity<OrderState, OrderClosed>
{
    private readonly ISomeService _service;

    public OrderClosedActivity(
        ISomeService service)
    {
        _service = service;
    }

    public async Task Execute(
        BehaviorContext<OrderState, OrderClosed> context,
        IBehavior<OrderState, OrderClosed> next)
    {
        await _service.OnOrderClosed(context.Saga.CorrelationId);

        // always call the next activity in the behavior
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<OrderState, OrderClosed, TException> context,
        IBehavior<OrderState, OrderClosed> next) where TException : Exception
    {
        // always call the next activity in the behavior
        return next.Faulted(context);
    }

    public void Probe(
    ProbeContext context)
    {
        context.CreateScope("publish-order-closed");
    }

    public void Accept(
        StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }
}
