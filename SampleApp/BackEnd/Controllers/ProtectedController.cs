using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProtectedController : ControllerBase
{

    [HttpGet("/api/protectedforCommonusers")]
    [Authorize]
    public IActionResult GetProtectedData()
    {
        return Ok("This is a protected route");
    }

    [HttpGet("/api/protectedforAdmins")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetProtectedDataForAdmins()
    {
        return Ok("This is a protected route for Admins");
    }
}
