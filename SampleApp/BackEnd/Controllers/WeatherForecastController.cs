using System.Collections.Generic;
using BackEnd.Logic.Queries;
using BackEnd.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleApp.BackeEnd.Logic.Queries;

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
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<WeatherForecast>> Get([FromQuery] ReadWeatherForecast request)
        {
            if (ModelState.IsValid)
            {
                var WeatherForecast = await _mediator.Send(new WeatherForecastQuery { Days = request.Days });
                return WeatherForecast != null ? Ok(WeatherForecast.Order()) : NoContent();
                //the order of the data is managed in the Controller layer (but it can be done in the Logic layer too, depend on the bussiness rules)
            }
            return BadRequest(ModelState);
        }
        /// <summary>
        /// Get the weather forecast for the next {days} days with an IASyncEnumerable
        /// </summary>
        /// <param name="request">request of the API</param>
        /// <returns>List of weather forecast</returns>
        [HttpGet("IASyncEnumerableVersion")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IAsyncEnumerable<WeatherForecast>>> GetIAsyncIEnumerable([FromQuery] ReadWeatherForecast request)
        {
            if (ModelState.IsValid)
            {
                var WeatherForecast = await _mediator.Send(new WeatherForecastQueryIAsyncEnumerable { Days = request.Days });
                return WeatherForecast != null ? Ok(WeatherForecast) : NoContent();
            }
            return BadRequest(ModelState);
        }

    }
}