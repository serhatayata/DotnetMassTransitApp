using DotnetMassTransitApp.Consumer.Contracts;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Consumer.Models;

public interface IOrderSubmitter
{
    Task Process(SubmitOrder order);
}
