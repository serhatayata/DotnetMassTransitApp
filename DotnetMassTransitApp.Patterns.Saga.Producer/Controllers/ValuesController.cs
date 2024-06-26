﻿using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Contracts;
using Shared.Queue.Events;
using Shared.Queue.Requests;
using Shared.Queue.Saga.Contracts;

namespace DotnetMassTransitApp.Patterns.Saga.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IRequestClient<RequestOrderCancellation> _cancelOrderRequestClient;

    public ValuesController(
        ISendEndpointProvider sendEndpointProvider,
        IRequestClient<RequestOrderCancellation> cancelOrderRequestClient)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _cancelOrderRequestClient = cancelOrderRequestClient;
    }

    [HttpGet("submit-order")]
    public async Task<IActionResult> SubmitOrderMethod()
    {
        var orderId = new Guid("4c02de9a-bcaa-4824-a004-ebbd4e0ff1c9");
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new("queue:submit-order"));

        await endpoint.Send<SubmitOrder>(
        new SubmitOrder()
        {
            OrderId = orderId,
            OrderDate = DateTime.Now,
            OrderAmount = 100,
            OrderNumber = "123123",
            OrderItems = new OrderItem[]
            {
               new()
               {
                   OrderId = orderId,
                   ItemNumber = "1123"
               }
            }
        });

        return Ok();            
    }

    [HttpGet("order-accepted")]
    public async Task<IActionResult> OrderAcceptedMethod()
    {
        var orderId = new Guid("4c02de9a-bcaa-4824-a004-ebbd4e0ff1c9");
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new("queue:order-accepted"));

        await endpoint.Send<OrderAccepted>(
        new OrderAccepted()
        {
            OrderId = orderId,
            CompletionTime = TimeSpan.FromSeconds(10)
        });

        return Ok();
    }

    [HttpGet("external-order-submitted")]
    public async Task<IActionResult> ExternalOrderSubmitted()
    {
        var orderNumber = "606a7b67-b8e5-4111-94a6-9f47fc280a22";
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new("queue:external-order-submitted"));

        await endpoint.Send<ExternalOrderSubmitted>(
        new ExternalOrderSubmitted()
        {
            OrderNumber = orderNumber
        });

        return Ok();
    }

    [HttpGet("order-cancellation-request")]
    public async Task<IActionResult> OrderCancellationRequest()
    {
        try
        {
            // V1

            //var message = new RequestOrderCancellation() { OrderId = new Guid("7955408b-6142-4d1f-9a96-510c82699ee9") };
            //var response = await _cancelOrderRequestClient.GetResponse<RequestOrderCancellation, OrderNotFound>(
            //    message: message);

            //if (response.Is(out Response<RequestOrderCancellation> canceled))
            //{
            //    return Ok();
            //}
            //else if (response.Is(out Response<OrderNotFound> responseB))
            //{
            //    return NotFound();
            //}

            //return BadRequest();

            // V2

            var message = new RequestOrderCancellation() { OrderId = new Guid("2fe4c617-e36f-4d4a-b413-a95c08338f50") };
            var response = await _cancelOrderRequestClient.GetResponse<RequestOrderCancellation, OrderNotFound, OrderCanceled>(
                message: message);

            if (response.Is(out Response<OrderCanceled> canceled))
            {
                return Ok();
            }
            else if (response.Is(out Response<OrderNotFound> responseB))
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

    [HttpGet("order-completed")]
    public async Task<IActionResult> OrderCompletedMethod()
    {
        var orderId = new Guid("2ba0a590-4320-4939-984e-b23ff813e75c");
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new("queue:order-completed"));

        await endpoint.Send<OrderCompleted>(
        new OrderCompleted()
        {
            OrderId = orderId
        });

        return Ok();
    }

    [HttpGet("create-order")]
    public async Task<IActionResult> CreateOrderMethod()
    {
        var correlationId = Guid.NewGuid();
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new("queue:create-order"));
        
        await endpoint.Send<CreateOrder>(
        new CreateOrder()
        {
            CorrelationId = correlationId
        });

        return Ok();
    }

    [HttpGet("order-completion-timeout-expired")]
    public async Task<IActionResult> OrderCompletionTimeoutExpiredMethod()
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new("queue:order-completion-timeout-expired"));

        await endpoint.Send<OrderCompletionTimeoutExpired>(
        new OrderCompletionTimeoutExpired()
        {
            OrderId = new Guid("05e302Da-abac-4e1c-9a4b-a347e7b357f8")
        }); ;

        return Ok();
    }
}
