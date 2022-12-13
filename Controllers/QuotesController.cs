using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using HeiLiving.Quotes.Api.Models;
using HeiLiving.Quotes.Api.Services;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace HeiLiving.Quotes.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private IQuotesService _calculatorService;
        private IUserService _userService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private IConverter _converter;
        private readonly ILogger _logger;

        public QuotesController(IQuotesService calculatorService, IUserService userService, IStringLocalizer<SharedResource> localizer, IConverter converter, ILogger<QuotesController> logger)
        {
            _calculatorService = calculatorService;
            _userService = userService;
            _localizer = localizer;
            _converter = converter;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Calculate([FromBody] QuoteInputViewModel input)
        {
            _logger.LogInformation($"GET Calculate. Input model: {JsonSerializer.Serialize(input)}");

            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonSerializer.Serialize(serializableModelState);
                _logger.LogError($"GET Calculate. Bad Model State: {modelStateJson}", modelStateJson);

                return BadRequest(ModelState);
            }

            try
            {
                var result = _calculatorService.Calculate(input);
                if (result == null)
                {
                    _logger.LogError($"GET Calculate. QuotesService unable to calculate");

                    return BadRequest();
                }

                _logger.LogInformation($"GET Calculate. Result model: {JsonSerializer.Serialize(result)}");

                return Ok(result);
            }
            catch (UnauthorizedAccessException exception)
            {
                return Unauthorized(exception.Message);
            }
        }

        [HttpPost("summary")]
        public IActionResult ExportSummaryAsContent([FromBody] QuoteInputViewModel input)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return BadRequest();
            }
            var userId = new Guid(identity.FindFirst("id").Value);

            _logger.LogInformation($"GET Summary. Input model: {JsonSerializer.Serialize(input)}");

            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonSerializer.Serialize(serializableModelState);
                _logger.LogError($"GET Summary. Bad Model State: {modelStateJson}", modelStateJson);

                return BadRequest(ModelState);
            }

            try
            {
                var result = _calculatorService.Calculate(input);

                if (result == null)
                {
                    _logger.LogError($"GET Calculate. QuotesService unable to calculate");

                    return BadRequest();
                }
                _logger.LogInformation($"GET Calculate. Result model: {JsonSerializer.Serialize(result)}");

                var content = _calculatorService.ExportSummaryToPdf(input, result, userId);

                if (content == null)
                {
                    _logger.LogError($"GET Summary. QuotesService unable to generate PDF summary");

                    return BadRequest();
                }

                var pdf = _converter.Convert(content);

                // ToDo. Make it async
                if (input.Customer.TrackInCRM)
                {
                    _calculatorService.CreateUpdateHubspotDeal(input, result, userId);
                }

                return new FileContentResult(pdf, new MediaTypeHeaderValue("application/pdf"));
            }
            catch (UnauthorizedAccessException exception)
            {
                return Unauthorized(exception.Message);
            }
        }
    }
}