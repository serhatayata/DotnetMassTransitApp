namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Services;

public interface ISomeService
{
    Task OnOrderClosed(Guid orderId);
    Task SomethingGlobal(Guid orderId);
}
