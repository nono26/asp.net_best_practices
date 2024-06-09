using AutoMapper;
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
    private readonly IMapper _mapper;

    public AuthController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new LoginQuery { Username = request.Username, Password = request.Password });
            var token = _mapper.Map<TokenResourcesDto>(response);
            return token != null ? Ok(token) : Unauthorized();//Or BadRequest() for security purpose
        }
        return BadRequest(ModelState);
    }
}