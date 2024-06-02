using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleApp.BackEnd.Models;

namespace SampleApp.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongRunningProcessController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LongRunningProcessController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Start a long running process
        /// </summary>
        /// <param name="request">request of the API</param>
        /// <returns></returns>
        [HttpPost("StartLongRunningProcess")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> StartLongRunningProcess([FromBody] StartLongRunningProcess request)
        {
            if (ModelState.IsValid)
            {
                //var result = await _mediator.Send(new LongRunningProcessCommand { Request = request });
                var processId = 123;
                var result = await Task.FromResult(processId);
                //return a 202 status with a location header that the client can use to check the status of the long running process
                return AcceptedAtAction(nameof(GetLongRunningProcessStatus), new { processId = processId }, result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get a long running process status
        /// </summary>
        /// <param name="request">request of the API</param>
        /// <returns></returns>
        [HttpGet(nameof(GetLongRunningProcessStatus))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        public async Task<IActionResult> GetLongRunningProcessStatus([FromQuery] GetLongRunningProcessStatus request)
        {
            if (ModelState.IsValid)
            {
                if (request.ProcessId == 123)
                {
                    var result = await Task.FromResult("Running");
                    return Ok(result);
                }
                else
                {
                    //return 303 status with a location header that the client can use to check the status of the long running process
                    return StatusCode(303, new { Location = string.Concat("/", nameof(GetLongRunningProcessStatus), "/", request.ProcessId, "/result") });
                }
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get the result of a long running process
        /// </summary>
        /// <param name="request">request of the API</param>
        /// <returns></returns>
        [HttpGet("GetLongRunningProcessResult/{processId}/result")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> GetLongRunningProcessResult([FromRoute] int processId)
        {
            if (ModelState.IsValid)
            {
                if (processId == 123)
                {
                    var result = await Task.FromResult("Success");
                    return Ok(result);
                }
            }
            return BadRequest(ModelState);
        }
    }
}