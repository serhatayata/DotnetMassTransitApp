using MassTransit;

namespace DotnetMassTransitApp.Exception.Models;

public interface Fault<T>
    where T : class
{
    Guid FaultId { get; }
    Guid? FaultedMessageId { get; }
    DateTime Timestamp { get; }
    ExceptionInfo[] Exceptions { get; }
    HostInfo Host { get; }
    T Message { get; }
}