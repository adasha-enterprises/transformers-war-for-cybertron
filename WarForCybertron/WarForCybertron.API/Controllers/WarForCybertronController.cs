using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WarForCybertron.Model;
using WarForCybertron.Model.DTO;
using WarForCybertron.Service.Interfaces;

namespace WarForCybertron.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class WarForCybertronController : ControllerBase
    {
        private readonly ILogger<WarForCybertronController> _logger;
        private readonly IWarForCybertronService _warForCybertronService;

        /// <summary>
        /// Constructor for the WarForCybertron controller
        /// </summary>
        /// <param name="warForCybertronService"></param>
        /// <param name="loggerFactory"></param>
        public WarForCybertronController(
        IWarForCybertronService warForCybertronService,
            ILoggerFactory loggerFactory
        )
        {
            _warForCybertronService = warForCybertronService;
            _logger = loggerFactory.CreateLogger<WarForCybertronController>();
        }

        // GET: api/warforcybertron
        /// <summary>
        /// Default get method to get all Transformers
        /// </summary>
        /// <returns>A list of all Transformer DTO objects</returns>
        /// <response code="200" cref="OkObjectResult">OkObjectResult(List&gt;TransformerDTO&lt; faqs)</response>
        /// <response code="400" cref="BadRequestObjectResult">BadRequest(new { string message })</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<List<TransformerDTO>>> GetAllTransformers()
        {
            string message;

            try
            {
                var serviceResponse = await _warForCybertronService.GetTransformers(null, false);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse.ResponseEntity);
                }
                else
                {
                    message = serviceResponse.ResponseMessage;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to get Transformers";
            }

            return BadRequest(new { message });
        }

        // POST: api/warforcybertron
        /// <summary>
        /// Post method to create a Transformer
        /// </summary>
        /// <param name="transformerDTO">The Transformer DTO to be created</param>
        /// <returns>The created transformer DTO object</returns>
        /// <response code="201" cref="CreatedAtActionResult">CreatedAtAction("CreateTransformer", new { Transformer = TransformerDTO })</response>
        /// <response code="400" cref="BadRequestObjectResult">BadRequest(new { string message, TransformerDTO transformerDTO })</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<TransformerDTO>> CreateTransformer(TransformerDTO transformerDTO)
        {
            string message;

            try
            {
                var serviceResponse = await _warForCybertronService.CreateTransformer(transformerDTO);

                if (serviceResponse.IsSuccess)
                {
                    return CreatedAtAction("CreateTransformer", new { Transformer = serviceResponse.ResponseEntity });
                }
                else
                {
                    message = serviceResponse.ResponseMessage;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, transformerDTO);
                message = "Unable to create Transformer";
            }

            return BadRequest(new { message, transformerDTO });
        }

        // GET: api/warforcybertron/dae5fa7b-f43d-e911-a1d9-186590cd8cde
        /// <summary>
        /// Get method to retrieve a Transformer
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The transformer DTO object that corresponds to the specified id</returns>
        /// <response code="200" cref="OkObjectResult">OkObjectResult(TransformerDTO transformerDTO)</response>
        /// <response code="400" cref="BadRequestObjectResult">BadRequest(new { string message, Guid id })</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransformerDTO>> GetTransformer(Guid id)
        {
            string message;

            try
            {
                var serviceResponse = await _warForCybertronService.GetTransformer(id);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse.ResponseEntity);
                }
                else
                {
                    message = serviceResponse.ResponseMessage;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, id);
                message = "Unable to get Transformer";
            }

            return BadRequest(new { message, id });
        }

        // PUT: api/warforcybertron/dae5fa7b-f43d-e911-a1d9-186590cd8cde
        /// <summary>
        /// Put method to update a Transformer
        /// </summary>
        /// <param name="id">The id of the Transformer to be updated</param>
        /// <param name="transformer">The Transformer DTO object to be updated</param>
        /// <returns>The updated Transformer DTO object included in an action result</returns>
        /// <response code="200" cref="OkObjectResult">OkObjectResult(TransformerDTO transformerDTO)</response>
        /// <response code="400" cref="BadRequestObjectResult">BadRequest(new { string message, TransformerDTO transformerDTO })</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}")]
        public async Task<ActionResult<TransformerDTO>> UpdateTransformer(Guid id, [FromBody] TransformerDTO transformer)
        {
            string message;

            try
            {
                if (id != transformer.Id)
                {
                    return BadRequest(transformer);
                }

                var serviceResponse = await _warForCybertronService.UpdateTransformer(transformer);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(transformer);
                }
                else
                {
                    message = serviceResponse.ResponseMessage;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, transformer);
                message = "Unable to update transformer";
            }

            return BadRequest(new { message, transformer });
        }

        // DELETE: api/warforcybertron/dae5fa7b-f43d-e911-a1d9-186590cd8cde
        /// <summary>
        /// Delete method to remove a Transformer
        /// </summary>
        /// <response code="200" cref="OkObjectResult">OkObjectResult()</response>
        /// <param name="id">The id of the Transformer to be deleted</param>
        /// <response code="200" cref="OkObjectResult">OkObjectResult()</response>
        /// <response code="400" cref="BadRequestObjectResult">BadRequest(new { string message, Guid id })</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWishlist(Guid id)
        {
            try
            {
                if (await _warForCybertronService.DeleteTransformer(id))
                {
                    return Ok();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, id);
            }

            return BadRequest(new { message = "Unable to delete transformer", id });
        }

        // GET: api/warforcybertron/autobots
        /// <summary>
        /// Get method to get all Autobot Transformers
        /// </summary>
        /// <returns>A list of all Autobot Transformer DTO objects</returns>
        /// <response code="200" cref="OkObjectResult">OkObjectResult(List&gt;TransformerDTO&lt; faqs)</response>
        /// <response code="400" cref="BadRequestObjectResult">BadRequest(new { string message })</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("autobots")]
        public async Task<ActionResult<List<TransformerDTO>>> GetAutobots()
        {
            string message;

            try
            {
                var serviceResponse = await _warForCybertronService.GetTransformers(Allegiance.AUTOBOT, false);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse.ResponseEntity);
                }
                else
                {
                    message = serviceResponse.ResponseMessage;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to get Autobots";
            }

            return BadRequest(new { message });
        }

        // GET: api/warforcybertron/decepticons
        /// <summary>
        /// Get method to get all Decepticon Transformers
        /// </summary>
        /// <returns>A list of all Decepticon Transformer DTO objects</returns>
        /// <response code="200" cref="OkObjectResult">OkObjectResult(List&gt;TransformerDTO&lt; faqs)</response>
        /// <response code="400" cref="BadRequestObjectResult">BadRequest(new { string message })</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("decepticons")]
        public async Task<ActionResult<List<TransformerDTO>>> GetDecepticons()
        {
            string message;

            try
            {
                var serviceResponse = await _warForCybertronService.GetTransformers(Allegiance.DECEPTICON, false);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse.ResponseEntity);
                }
                else
                {
                    message = serviceResponse.ResponseMessage;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                message = "Unable to get Decepticons";
            }

            return BadRequest(new { message });
        }

        // GET: api/warforcybertron/dae5fa7b-f43d-e911-a1d9-186590cd8cde/score
        /// <summary>
        /// Get method to retrieve a Transformer's score
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The transformer DTO object that corresponds to the specified id</returns>
        /// <response code="200" cref="OkObjectResult">OkObjectResult(TransformerDTO transformerDTO)</response>
        /// <response code="400" cref="BadRequestObjectResult">BadRequest(new { string message, Guid id })</response>
        [HttpGet("{id}/score")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransformerDTO>> GetTransformerScore(Guid id)
        {
            string message;

            try
            {
                var score = await _warForCybertronService.GetOverallScore(id);

                return Ok(score);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, id);
                message = "Unable to get Transformer score";
            }

            return BadRequest(new { message, id });
        }
    }
}