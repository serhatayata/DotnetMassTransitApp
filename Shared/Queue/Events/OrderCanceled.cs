﻿namespace Shared.Queue.Events;

public class OrderCanceled
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
}
