using DotnetMassTransitApp.Patterns.Saga.StateMachine;
using DotnetMassTransitApp.Patterns.Saga.StateMachine.Infrastructure.Contexts;
using DotnetMassTransitApp.Patterns.Saga.StateMachine.Services;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Shared.Queue.Contracts;
using Shared.Queue.Events;
using Shared.Queue.Saga;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddScoped<ISomeService, SomeService>();

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(cfg =>
        {
            cfg.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

            cfg.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
            {
                var connectionString = configuration.GetConnectionString("OrderState");
                builder.UseSqlServer(connectionString, m =>
                {
                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                });
            });

            cfg.LockStatementProvider = new SqlServerLockStatementProvider();
            cfg.ConcurrencyMode = ConcurrencyMode.Pessimistic;
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitmqConn = configuration.GetConnectionString("RabbitMQ");

        cfg.Host(rabbitmqConn);

        cfg.Message<ProcessOrder>(x =>
        {
            x.SetEntityName("process-order-exchange");
        });

        cfg.Message<OrderCompleted>(x =>
        {
            x.SetEntityName("order-completed-exchange");
        });



        cfg.ReceiveEndpoint("submit-order", r =>
        {
            r.ConfigureSaga<OrderState>(context);
        });

        cfg.ReceiveEndpoint("order-accepted", r =>
        {
            r.ConfigureSaga<OrderState>(context);
        });

        cfg.ReceiveEndpoint("external-order-submitted", r =>
        {
            r.ConfigureSaga<OrderState>(context);
        });

        cfg.ReceiveEndpoint("order-cancellation-requested", r =>
        {
            r.ConfigureSaga<OrderState>(context);
        });

        cfg.ReceiveEndpoint("order-completed", r =>
        {
            r.ConfigureSaga<OrderState>(context);
        });

        cfg.ReceiveEndpoint("create-order", r =>
        {
            r.ConfigureSaga<OrderState>(context);
        });

        cfg.ReceiveEndpoint("order-completion-timeout-expired", r =>
        {
            r.ConfigureSaga<OrderState>(context);
        });
    });
});

var serviceProvider = builder.Services.BuildServiceProvider();
var context = serviceProvider.GetRequiredService<DbContext>();

context.Database.Migrate();

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
