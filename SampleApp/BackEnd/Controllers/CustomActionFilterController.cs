
using Microsoft.AspNetCore.Mvc;
using SampleApp.BackEnd.Attributes;
using SampleApp.BackEnd.Models;

namespace SampleApp.BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerActionFilterController : ControllerBase
{
    [HttpPut]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public ActionResult Index([FromBody] TurnOnLightsREST request) =>
        Ok(true);
}