using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Infrastructure.Contexts;

public class OrderStateDbContextFactory : IDesignTimeDbContextFactory<OrderStateDbContext>
{
    public OrderStateDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json",
                         optional: false,
                         reloadOnChange: true)
            .Build();

        var builder = new DbContextOptionsBuilder<OrderStateDbContext>();

        var connectionString = configuration.GetConnectionString("OrderState");

        builder.UseSqlServer(connectionString, m =>
        {
            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
        });

        return new OrderStateDbContext(builder.Options);
    }
}
