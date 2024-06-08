using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using SampleApp.BackEnd.Logic.Queries;
using SampleApp.BackEnd.Models;

namespace SampleApp.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
    {
        if (ModelState.IsValid)
        {
            var token = await _mediator.Send(new LoginQuery { Username = request.Username, Password = request.Password });
            return token != null ? Ok(token) : Unauthorized();//Or BadRequest() for security purpose
        }
        return BadRequest(ModelState);
    }
}