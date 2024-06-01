using DotnetMassTransitApp.Patterns.Saga.StateMachine.Infrastructure.StateMappings;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Infrastructure.Contexts;

public class OrderStateDbContext : SagaDbContext
{
    private readonly IConfiguration _configuration;

    public OrderStateDbContext(
        DbContextOptions options, 
        IConfiguration configuration)
    : base(options)
    {
        _configuration = configuration;
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderStateMap(); }
    }
}
