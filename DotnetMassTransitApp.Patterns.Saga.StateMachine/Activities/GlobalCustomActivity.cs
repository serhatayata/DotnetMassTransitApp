using DotnetMassTransitApp.Patterns.Saga.StateMachine.Services;
using MassTransit;
using Shared.Queue.Saga;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Activities;

public class GlobalCustomActivity
    : IStateMachineActivity<OrderState>
{
    private readonly ISomeService _service;

    public GlobalCustomActivity(
        ISomeService service)
    {
        _service = service;
    }

    public async Task Execute(
        BehaviorContext<OrderState> context, 
        IBehavior<OrderState> next)
    {
        await _service.SomethingGlobal(context.Saga.CorrelationId);

        // always call the next activity in the behavior
        await next.Execute(context).ConfigureAwait(false);
    }

    public async Task Execute<T>(
        BehaviorContext<OrderState, T> context, 
        IBehavior<OrderState, T> next) where T : class
    {
        await _service.SomethingGlobal(context.Saga.CorrelationId);

        // always call the next activity in the behavior
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<OrderState, TException> context, 
        IBehavior<OrderState> next) where TException : Exception
    {
        // always call the next activity in the behavior
        return next.Faulted(context);
    }

    public Task Faulted<T, TException>(
        BehaviorExceptionContext<OrderState, T, TException> context, 
        IBehavior<OrderState, T> next)
        where T : class
        where TException : Exception
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
