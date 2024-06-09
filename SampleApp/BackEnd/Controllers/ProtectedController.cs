using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleApp.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProtectedController : ControllerBase
{

    [HttpGet("/api/protectedforCommonuusers")]
    [Authorize]
    public IActionResult GetProtectedData()
    {
        return Ok("This is a protected route");
    }
}
