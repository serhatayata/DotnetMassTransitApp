using DotnetMassTransitApp.Patterns.Saga.StateMachine;
using MassTransit;
using Shared.Queue.Saga;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .InMemoryRepository();
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
