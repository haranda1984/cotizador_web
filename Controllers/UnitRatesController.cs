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
    public class UnitRatesController : ControllerBase
    {
        private readonly IUnitRatesService _unitRatesService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UnitRatesController(IUnitRatesService unitRatesService, IStringLocalizer<SharedResource> localizer)
        {
            _unitRatesService = unitRatesService;
            _localizer = localizer;
        }

        // GET api/UnitRates
        [HttpGet]
        public async Task<IActionResult> GetAllUnitRatesAsync()
        {
            var unitRates = await _unitRatesService.GetAllUnitRatesAsync();

            if (unitRates == null)
            {
                return NotFound();
            }

            var viewModel = unitRates.Select(unitRates => new UnitRateViewModel
            {
                UnitId = unitRates.UnitId,
                CostPerNight = unitRates.CostPerNight,
                BuiltUpAreaCost = unitRates.BuiltUpAreaCost,
                TerraceAreaCost = unitRates.TerraceAreaCost,
            });

            return Ok(viewModel);
        }

        // GET api/UnitRates/5
        [HttpGet("{id}", Name = "UnitRateById")]
        public async Task<IActionResult> GetUnitRateByIdsAsync(Guid unitId, Guid rateId)
        {
            var unitRate = await _unitRatesService.GetUnitRateByIdsAsync(unitId, rateId);

            if (unitRate == null)
            {
                return NotFound();
            }

            var viewModel = new UnitRateViewModel
            {
                UnitId = unitRate.UnitId,
                CostPerNight = unitRate.CostPerNight,
                BuiltUpAreaCost = unitRate.BuiltUpAreaCost,
                TerraceAreaCost = unitRate.TerraceAreaCost,
            };

            return Ok(viewModel);
        }

            // GET api/UnitRates/{unitId}
        [HttpGet("{unitId}", Name = "UnitRatesByUniId")]
        public async Task<IActionResult> GetAllUnitRatesByUnitIdAsync(Guid unitId)
        {
            var unitRates = await _unitRatesService.GetAllUnitRatesByUnitIdAsync(unitId);

            if (unitRates == null)
            {
                return NotFound();
            }

            var viewModel = unitRates.Select(unitRate => new UnitRateViewModel
            {
                UnitId = unitRate.UnitId,
                CostPerNight = unitRate.CostPerNight,
                BuiltUpAreaCost = unitRate.BuiltUpAreaCost,
                TerraceAreaCost = unitRate.TerraceAreaCost,
            });

            return Ok(viewModel);
        }

        // POST api/UnitRates
        [HttpPost]
        public async Task<IActionResult> CreateUnitRate([FromBody] UnitRateViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["UnitRate object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var unitRate = new UnitRate
            {
                UnitId = resource.UnitId,
                CostPerNight = resource.CostPerNight,
                BuiltUpAreaCost = resource.BuiltUpAreaCost,
                TerraceAreaCost = resource.TerraceAreaCost,
            };

            await _unitRatesService.CreateUnitRateAsync(unitRate);

            return CreatedAtRoute("UnitRateById", new { UnitId = resource.UnitId}, resource);
        }

        // PUT api/UnitRates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnitRate([FromBody] UnitRateViewModel resource, Guid unitId, Guid rateId)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["UnitRates object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbUnitRate = await _unitRatesService.GetUnitRateByIdsAsync(unitId, rateId);
            if (dbUnitRate == null)
            {
                return NotFound();
            }

            var unitRate = new UnitRate
            {
                UnitId = resource.UnitId,
                CostPerNight = resource.CostPerNight,
                BuiltUpAreaCost = resource.BuiltUpAreaCost,
                TerraceAreaCost = resource.TerraceAreaCost,
            };

            await _unitRatesService.UpdateUnitRateAsync(dbUnitRate, unitRate);

            return NoContent();
        }

        // DELETE api/UnitRates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnitRate(Guid unitId, Guid rateId)
        {
            var unitRate = await _unitRatesService.GetUnitRateByIdsAsync(unitId, rateId);
            if (unitRate == null)
            {
                return NotFound();
            }

            await _unitRatesService.DeleteUnitRateAsync(unitRate);

            return NoContent();
        }
    }
}