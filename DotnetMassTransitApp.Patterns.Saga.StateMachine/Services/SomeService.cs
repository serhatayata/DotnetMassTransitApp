
namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Services;

public class SomeService : ISomeService
{
    public Task OnOrderClosed(Guid orderId)
    {
        return Task.CompletedTask;
    }

    public Task SomethingGlobal(Guid orderId)
    {
        return Task.CompletedTask;
    }
}
