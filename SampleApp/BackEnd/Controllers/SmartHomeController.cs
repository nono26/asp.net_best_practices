using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleApp.BackEnd.Attributes;
using SampleApp.BackEnd.Models;

namespace SampleApp.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SmartHomeController : ControllerBase
{
    private readonly IMediator _mediator;

    public SmartHomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// RPC Style to turn on the lights on
    /// It is not a REST approach. Rest is about all about the state, not the behavior
    /// we should avoid Post request because it is not idempotent
    // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/http-overview
    /// </summary>
    /// <param name="request">request of the API</param>
    /// <returns></returns>
    [HttpPost("TurnOnLights_RPC_Style")]
    public async Task<ActionResult<Boolean>> TurnOnLightsRPC([FromBody] TurnOnLightsRPC request)
    {
        if (ModelState.IsValid)
        {
            var result = await Task.FromResult(true);
            return Ok(result);
        }
        return BadRequest(ModelState);
    }

    /// <summary>
    /// Rest Style to turn on the lights on
    /// It is REST approach. Rest is about all about the state, not the behavior
    /// Here the desired state is sent to the server
    /// </summary>
    /// <param name="request">request of the API</param>
    /// <returns></returns>
    [HttpPut("TurnOnLights_REST_Style")]
    public async Task<ActionResult<Boolean>> TurnOnLightsREST([FromBody] TurnOnLightsREST request)
    {
        if (ModelState.IsValid)
        {
            if (request.Status == 1)
            {
                // Turn on the lights
            }
            else if (request.Status == 0)
            {
                // Turn off the lights
            }
            var result = await Task.FromResult(true);
            return Ok(result);
        }
        return BadRequest(ModelState);
    }
}