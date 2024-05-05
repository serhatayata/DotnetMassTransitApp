using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Contracts;
using Shared.Queue.Events;
using Shared.Queue.Requests;
using Shared.Queue.Responses;
using System.Threading;

namespace DotnetMassTransitApp.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly IRequestClient<FinalizeOrderRequest> _finalizeOrderRequestClient;
    private readonly IRequestClient<CheckOrderStatus> _checkOrderStatusRequestClient;
    private readonly IRequestClient<CancelOrder> _cancelOrderRequestClient;

    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IBus _bus;

    public ValuesController(
        ISendEndpointProvider sendEndpointProvider,
        IBus bus,
        IRequestClient<FinalizeOrderRequest> finalizeOrderRequestClient,
        IRequestClient<CheckOrderStatus> checkOrderStatusRequestClient,
        IRequestClient<CancelOrder> cancelOrderRequestClient)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _bus = bus;
        _finalizeOrderRequestClient = finalizeOrderRequestClient;
        _checkOrderStatusRequestClient = checkOrderStatusRequestClient;
        _cancelOrderRequestClient = cancelOrderRequestClient;
    }

    [HttpGet("send-endpoint")]
    public async Task<IActionResult> SendEndpointMethod()
    {
        try
        {
            string serviceAddress = "rabbitmq://localhost/submit-order-queue";
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(serviceAddress));
            //var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:input-queue"));

            var timeout = TimeSpan.FromSeconds(5);
            using var source = new CancellationTokenSource(timeout);

            await endpoint.Send(new SubmitOrder
            {
                OrderId = Guid.NewGuid()
            }, source.Token);

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("address-conventions")]
    public async Task<IActionResult> AddressConventionsMethod()
    {
        try
        {
            await _sendEndpointProvider.Send(
            new SubmitOrder
            {
                OrderId = Guid.NewGuid()
            });

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("address-conventions-bus")]
    public async Task<IActionResult> AddressConventionsBusMethod()
    {
        try
        {
            var timeout = TimeSpan.FromSeconds(5);
            using var source = new CancellationTokenSource(timeout);

            await _bus.Send(
            new SubmitOrder
            {
                OrderId = Guid.NewGuid()
            }, source.Token);

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("header")]
    public async Task<IActionResult> HeaderMethod()
    {
        try
        {
            //actually, that's a bad example since the request client already sets the message expiration, but you, get,
            //the, point.

            var zipkinTraceId = Guid.NewGuid();
            var zipkinSpanId = Guid.NewGuid();

            var response = await _finalizeOrderRequestClient.GetResponse<FinalizeOrderResponse>(new
            {
                __TimeToLive = 15000, // 15 seconds, or in this case, 15000 milliseconds
                __Header_X_B3_TraceId = zipkinTraceId,
                __Header_X_B3_SpanId = zipkinSpanId,
                OrderId = Guid.NewGuid(),
            });

            return Ok(response);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("variables")]
    public async Task<IActionResult> VariablesMethod()
    {
        try
        {
            await _sendEndpointProvider.Send(
            new SubmitOrder
            {
                OrderId = InVar.Id,
                OrderDate = InVar.Timestamp,
                OrderNumber = "18001",
                OrderAmount = 123.45m,
                OrderItems = new OrderItem[]
                {
                    new OrderItem { OrderId = InVar.Id, ItemNumber = "237" },
                    new OrderItem { OrderId = InVar.Id, ItemNumber = "762" }
                }
            });

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("send-headers")]
    public async Task<IActionResult> SendHeadersMethod()
    {
        try
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("rabbitmq://localhost/submit-order"));

            await endpoint.Send<SubmitOrder>(
            new SubmitOrder
            {
                OrderId = InVar.Id,
                OrderDate = InVar.Timestamp,
                OrderNumber = "18001",
                OrderAmount = 123.45m,
                OrderItems = new OrderItem[]
                {
                    new OrderItem { OrderId = InVar.Id, ItemNumber = "237" },
                    new OrderItem { OrderId = InVar.Id, ItemNumber = "762" }
                }
            }, context => context.FaultAddress = new Uri("rabbitmq://localhost/order_faults"));

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("send-notification-order")]
    public async Task<IActionResult> SendNotificationOrderMethod()
    {
        try
        {
            await _sendEndpointProvider.Send(
            new SendNotificationOrder
            {
                OrderId = Guid.NewGuid(),
                Email = "srht@mail.com"
            });

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// If the cancellationToken passed to GetResponse is canceled, the request client will stop waiting for a response.        However, the request message produced remains in the queue until it is consumed or the message time-to-live expires.    By default, the message time-to-live is set to the request timeout (which defaults to 30 seconds).
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("check-order-status")]
    public async Task<IActionResult> CheckOrderStatusMethod(CancellationToken cancellationToken)
    {
        try
        {
            var orderId = Guid.NewGuid();

            // 1
            Response response = await _checkOrderStatusRequestClient.GetResponse<OrderStatusResult, OrderNotFound>(
                new OrderStatusResult 
                { 
                    OrderId = orderId
                }, x => x.UseExecute(context => context.Headers.Set("tenant-id", "some-value")), cancellationToken);

            // 2
            //await _checkOrderStatusRequestClient.GetResponse<OrderStatusResult>(new
            //{
            //    orderId,
            //    __Header_Tenant_Id = "some-value"
            //}, cancellationToken);

            // Multiple response types 1
            //if (response.Is(out Response<OrderStatusResult> responseA))
            //{
            //    // do something with the order
            //}
            //else if (response.Is(out Response<OrderNotFound> responseB))
            //{
            //    // the order was not found
            //}

            // Multiple response types 2
            var accepted = response switch
            {
                (_, OrderStatusResult a) => true,
                (_, OrderNotFound b) => false,
                _ => throw new InvalidOperationException()
            };

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("accept-response-types")]
    public async Task<IActionResult> AcceptResponseTypesMethod(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _cancelOrderRequestClient.GetResponse<OrderCanceled, OrderNotFound, OrderAlreadyShipped>(new CancelOrder());

            if (response.Is(out Response<OrderCanceled> canceled))
            {
                return Ok();
            }
            else if (response.Is(out Response<OrderNotFound> responseB))
            {
                return NotFound();
            }
            else if (response.Is(out Response<OrderAlreadyShipped> responseC))
            {
                return NotFound();
            }

            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
}
