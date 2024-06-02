using BackEnd.Logic.Queries;
using BackEnd.Models;
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

        /// <summary>
        /// Get the weather forecast for the next {days} days
        /// </summary>
        /// <param name="request">request of the API</param>
        /// <returns>List of weather forecast</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), 200)]
        //[ProducesResponseType(typeof(ModelStateDictionary), 400)]
        public async Task<IActionResult> Get([FromQuery] ReadWeatherForecast request)
        {
            if (ModelState.IsValid)
            {
                var WeatherForecast = await _mediator.Send(new WeatherForecastQuery { Days = request.Days });
                return WeatherForecast != null ? Ok(WeatherForecast.Order()) : NoContent();
                //the order of the data is managed in the Controller layer (but it can be done in the Logic layer too, depend on the bussiness rules)
            }
            return BadRequest(ModelState);
        }
    }
}