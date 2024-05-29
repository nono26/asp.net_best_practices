using BackEnd.Logic.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly IMediator _mediator;

        public WeatherForecastController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var WeatherForecast = await _mediator.Send(new WeatherForecastQuery { Days = 5 });
            return WeatherForecast.OrderBy(x => x.Date);
        }
    }
}