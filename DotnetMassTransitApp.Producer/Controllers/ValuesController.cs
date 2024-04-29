﻿using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Contracts;
using Shared.Queue.Requests;
using Shared.Queue.Responses;

namespace DotnetMassTransitApp.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly IRequestClient<FinalizeOrderRequest> _finalizeOrderRequestClient;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IBus _bus;

    public ValuesController(
        ISendEndpointProvider sendEndpointProvider,
        IBus bus,
        IRequestClient<FinalizeOrderRequest> finalizeOrderRequestClient)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _bus = bus;
        _finalizeOrderRequestClient = finalizeOrderRequestClient;
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
                OrderId = "112233"
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
                OrderId = "112233"
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
                OrderId = "112233"
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


}
