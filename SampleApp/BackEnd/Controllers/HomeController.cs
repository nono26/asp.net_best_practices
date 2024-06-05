using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace SampleApp.BackEnd.Controllers;

public class HomeController : ControllerBase
{
    /// <summary> 
    /// Handle the error in production environment
    /// </summary>
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)] //Not including in swagger documentation 
    [Route("/error")]
    public IActionResult HandleError() =>
        Problem("An error occurred", statusCode: 500); //return a 500 status code with no detail for production environment


    /// <summary>
    /// Handle the error in development environment
    /// </summary>
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)] //Not including in swagger documentation
    [Route("/error-local-development")]
    public IActionResult HandleErrorLocalDevelopment([FromServices] IHostEnvironment hostEnvironnement)
    {
        if (hostEnvironnement.IsDevelopment())
        {
            return NotFound();
        }
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
        return Problem(exceptionHandlerFeature.Error.StackTrace, exceptionHandlerFeature.Error.Message); //return a 500 status code with detail for development environment
    }
}

