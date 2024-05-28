using MassTransit;
using Shared.Queue.Saga.Contracts;

namespace Shared.Queue.Saga;

public class OrderPaymentSaga :
    ISaga,
    InitiatedByOrOrchestrates<OrderInvoiced>
{
    public Guid CorrelationId { get; set; }

    public DateTime? InvoiceDate { get; set; }
    public decimal? Amount { get; set; }

    public async Task Consume(ConsumeContext<OrderInvoiced> context)
    {
        InvoiceDate = context.Message.Timestamp;
        Amount = context.Message.Amount;
    }
}
