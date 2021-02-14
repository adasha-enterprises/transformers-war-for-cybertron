using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        // GET: api/transformer
        /// <summary>
        /// Default method to get all Transformers
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

                if (string.IsNullOrEmpty(serviceResponse.ResponseMessage))
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

        // POST: api/transformer
        /// <summary>
        /// Post method to create a transformer
        /// </summary>
        /// <param name="transformerDTO">The transformer DTO to be created</param>
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

                if (string.IsNullOrEmpty(serviceResponse.ResponseMessage))
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
    }
}