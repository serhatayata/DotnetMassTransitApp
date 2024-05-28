using MassTransit;
using Shared.Queue.Contracts;
using Shared.Queue.Saga.Contracts;
using System.Linq.Expressions;

namespace Shared.Queue.Saga;

/// <summary>
/// When a CompleteOrder message is received by the saga's receive endpoint, the CorrelationId property is used to 
/// determine if an existing saga instance with that CorrelationId exists. If an existing instance is not found, 
/// the repository creates a new saga instance and calls the Consume method on the new instance. After the Consume 
/// method completes, the repository saves the newly created instance.
/// </summary>
public class OrderSaga : ISaga, InitiatedBy<CompleteOrder>
{
    public Guid CorrelationId { get; set; }

    public DateTime? CompleteDate { get; set; }
    public DateTime? AcceptDate { get; set; }

    public async Task Consume(ConsumeContext<CompleteOrder> context)
    {
        CompleteDate = context.Message.OrderDate;
    }
}

//public class OrderSaga :
//    ISaga,
//    InitiatedBy<SubmitOrder>,
//    Orchestrates<OrderAccepted>,
//{
//    public Guid CorrelationId { get; set; }

//    public DateTime? SubmitDate { get; set; }
//    public DateTime? AcceptDate { get; set; }

//    public async Task Consume(ConsumeContext<SubmitOrder> context) { ...}

//    public async Task Consume(ConsumeContext<OrderAccepted> context)
//    {
//        AcceptDate = context.Message.Timestamp;
//    }
//}

//public class OrderSaga :
//    ISaga,
//    InitiatedBy<SubmitOrder>,
//    Orchestrates<OrderAccepted>,
//    Observes<OrderShipped, OrderSaga>
//{
//    public Guid CorrelationId { get; set; }

//    public DateTime? SubmitDate { get; set; }
//    public DateTime? AcceptDate { get; set; }
//    public DateTime? ShipDate { get; set; }

//    public async Task Consume(ConsumeContext<SubmitOrder> context) { ...}
//    public async Task Consume(ConsumeContext<OrderAccepted> context) { ...}

//    public async Task Consume(ConsumeContext<OrderShipped> context)
//    {
//        ShipDate = context.Message.ShipDate;
//    }

//    public Expression<Func<OrderSaga, OrderShipped, bool>> CorrelationExpression =>
//        (saga, message) => saga.CorrelationId == message.OrderId;
//}