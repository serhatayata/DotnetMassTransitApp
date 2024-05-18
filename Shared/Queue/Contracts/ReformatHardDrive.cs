using MassTransit;

namespace Shared.Queue.Contracts;

[ExcludeFromTopology]
public interface ICommand
{
}

public class ReformatHardDrive : ICommand
{
    public int ProductId { get; set; }
}
