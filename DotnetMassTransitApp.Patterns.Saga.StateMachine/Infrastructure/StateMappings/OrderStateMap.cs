using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Queue.Saga;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Infrastructure.StateMappings;

public class OrderStateMap : SagaClassMap<OrderState>
{
    protected override void Configure(
        EntityTypeBuilder<OrderState> entity, 
        ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);

        //entity.Property(x => x.OrderDate);

        //entity.Property(x => x.RowVersion).IsRowVersion();
    }
}