using DotnetMassTransitApp.Patterns.Saga.StateMachine.Infrastructure.StateMappings;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Infrastructure.Contexts;

public class OrderStateDbContext : SagaDbContext
{

    public OrderStateDbContext(
        DbContextOptions options)
    : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderStateMap(); }
    }
}
