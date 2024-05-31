using DotnetMassTransitApp.Patterns.Saga.StateMachine;
using DotnetMassTransitApp.Patterns.Saga.StateMachine.Services;
using MassTransit;
using Shared.Queue.Saga;
using static MassTransit.Logging.OperationName;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .InMemoryRepository();
});

builder.Services.AddScoped<ISomeService, SomeService>();

// Sagas are added inside the AddMassTransit configuration using any of the following methods.

//builder.Services.AddMassTransit(cfg =>
//{
//    cfg.AddSaga<MySaga>();
//    cfg.AddSaga(typeof(MySaga));

//    // Adds a saga with a matching saga definition
//    cfg.AddSaga<MySaga, MySagaDefinition>();
//    cfg.AddSaga(typeof(MySaga), typeof(MySagaDefinition));

//    // Adds a saga with a matching saga definition
//    // and configures the saga pipeline.
//    cfg.AddSaga<MySaga, MySagaDefinition>(cfg =>
//    {
//        cfg.ConcurrentMessageLimit = 8;
//    });

//    // Adds the specified sagas and saga definitions.
//    // When saga definitions are included they will 
//    // be added with the matching saga type.
//    // AddSagas(params Type[] types);
//    cfg.AddSagas(typeof(MySaga), typeof(MyOtherSagaDefinition));

//    // Adds all sagas and saga definitions in the specified 
//    // an assembly or assemblies. 
//    // AddSagas(params Assembly[] assemblies);
//    cfg.AddSagas(typeof(Program).Assembly)

//    // Adds the sagas and any matching saga definitions 
//    // in the specified an assembly or assemblies that pass
//    // the filter. The filter is only called for saga types.
//    // AddSagas(Func<Type, bool> filter, params Assembly[] assemblies);
//    cfg.AddSagas(
//        t => t.Name.StartsWith("S"),
//        typeof(Program).Assembly
//    )
//});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
