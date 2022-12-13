using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using HeiLiving.Quotes.Api.Services;
using HeiLiving.Quotes.Api.Models;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Controllers
{
    [Authorize(Roles = "super_admin,admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RatesController : ControllerBase
    {
        private readonly IRatesService _ratesService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RatesController(IRatesService ratesService, IStringLocalizer<SharedResource> localizer)
        {
            _ratesService = ratesService;
            _localizer = localizer;
        }

        // GET api/rates
        [HttpGet]
        public async Task<IActionResult> GetAllRatesAsync()
        {
            var rates = await _ratesService.GetAllRatesAsync();

            if (rates == null)
            {
                return NotFound();
            }

            var viewModel = rates.Select(expense => new RateViewModel
            {
                Id = expense.Id,
                Name = expense.Name,
                Description = expense.Description
            });

            return Ok(viewModel);
        }

        // GET api/rates/5
        [HttpGet("{id}", Name = "RateById")]
        public async Task<IActionResult> GetRateByIdAsync(Guid id)
        {
            var rate = await _ratesService.GetRateByIdAsync(id);

            if (rate == null)
            {
                return NotFound();
            }

            var viewModel = new RateViewModel
            {
                Id = rate.Id,
                Name = rate.Name,
                Description = rate.Description
            };

            return Ok(viewModel);
        }        

        // GET api/rates/{name}
        [HttpGet("{name}", Name = "RateByName")]
        public async Task<IActionResult> GetRateByNameAsync(string name)
        {
            var rate = await _ratesService.GetRateByNameAsync(name);

            if (rate == null)
            {
                return NotFound();
            }

            var viewModel = new RateViewModel
            {
                Id = rate.Id,
                Name = rate.Name,
                Description = rate.Description
            };

            return Ok(viewModel);
        } 

        // POST api/rates
        [HttpPost]
        public async Task<IActionResult> CreateRate([FromBody] RateViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Rate object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var expense = new Rate
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
            };

            await _ratesService.CreateRateAsync(expense);

            return CreatedAtRoute("RateById", new { id = resource.Id }, resource);
        }

        // PUT api/rates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRate([FromBody] RateViewModel resource, Guid id)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Rate object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbRate = await _ratesService.GetRateByIdAsync(id);
            if (dbRate == null)
            {
                return NotFound();
            }

            var rate = new Rate
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
            };

            await _ratesService.UpdateRateAsync(dbRate, rate);

            return NoContent();
        }

        // DELETE api/Rate/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRate(Guid id)
        {
            var rate = await _ratesService.GetRateByIdAsync(id);
            if (rate == null)
            {
                return NotFound();
            }

            await _ratesService.DeleteRateAsync(rate);

            return NoContent();
        }
    }
}