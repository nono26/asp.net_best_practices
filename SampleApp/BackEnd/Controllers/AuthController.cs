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
    public async Task<ActionResult<TokenResourcesDto>> Login([FromBody] LoginRequest request)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new LoginQuery { Username = request.Username, Password = request.Password });
            var token = _mapper.Map<TokenResourcesDto>(response);
            return token != null ? Ok(token) : Unauthorized();//Or BadRequest() for security purpose
        }
        return BadRequest(ModelState);
    }

    [HttpPost("refreshToken")]
    public async Task<ActionResult<TokenResourcesDto>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new RefreshLoginQuery { Email = request.Email, RefreshToken = request.RefreshToken });
            var token = _mapper.Map<TokenResourcesDto>(response);
            return token != null ? Ok(token) : Unauthorized();//Or BadRequest() for security purpose
        }
        return BadRequest(ModelState);
    }

    [HttpPost("revokeRefreshToken")]
    public async Task<ActionResult> Revoke([FromBody] RevokeTokenResource request)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new RevokeRefreshTokenQuery(request.RefreshToken, request.Email));
            return response ? Ok() : Unauthorized();//Or BadRequest() for security purpose
        }
        return BadRequest(ModelState);
    }

}