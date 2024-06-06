using System.Collections.Generic;
using BackEnd.Logic.Queries;
using BackEnd.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleApp.BackEnd.Attributes;
using SampleApp.BackEnd.Logic.Queries;

namespace SampleApp.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ResponseHeader("Filder-header", "Filder-Value")]//add a header to the response for each request #filterAttribut_1
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
        [ProducesResponseType(200)]//for swagger documentation
        [ProducesResponseType(400)]//for swagger documentation
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]//Cache configuration, it adds in header : cache-control: public,max-age=10
        [ResponseHeader("Another-filter-header", "Another Filter Value")]//add a header to the response to this request #filterAttribut_2
        public async Task<ActionResult<WeatherForecast>> Get([FromQuery] ReadWeatherForecast request)
        {
            if (ModelState.IsValid)
            {
                var WeatherForecast = await _mediator.Send(new WeatherForecastQuery { Days = request.Days });
                return WeatherForecast != null ? Ok(WeatherForecast.OrderBy(x => x.Date)) : NoContent();
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
        [ProducesResponseType(200)]//for swagger documentation
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