﻿namespace Shared.Queue.Contracts;

public class OrderAccepted
{
    public Guid OrderId { get; set; }
    public TimeSpan CompletionTime { get; set; }
}
