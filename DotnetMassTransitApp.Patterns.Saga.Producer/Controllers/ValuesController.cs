﻿using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Contracts;
using Shared.Queue.Events;

namespace DotnetMassTransitApp.Patterns.Saga.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ValuesController(
        ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpGet("submit-order")]
    public async Task<IActionResult> SubmitOrderMethod()
    {
        var orderId = new Guid("74ce06a3-86e6-4844-8e74-09833b41f3e7");
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
        var orderId = new Guid("74ce06a3-86e6-4844-8e74-09833b41f3e7");
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
}
