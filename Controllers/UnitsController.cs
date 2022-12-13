using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using HeiLiving.Quotes.Api.Services;
using HeiLiving.Quotes.Api.Models;
using HeiLiving.Quotes.Api.Entities;
using System.Collections.Generic;
using HeiLiving.Quotes.Api.Helpers;

namespace HeiLiving.Quotes.Api.Controllers
{
    [Authorize(Roles = "super_admin,admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitService _unitService;
        private readonly IUnitEquipmentsService _unitEquipmentService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UnitsController(IUnitService unitService, IUnitEquipmentsService unitEquipmentService, IStringLocalizer<SharedResource> localizer)
        {
            _unitService = unitService;
            _unitEquipmentService = unitEquipmentService;
            _localizer = localizer;
        }

        // GET api/units/5
        [HttpGet("{id}", Name = "UnitById")]
        public async Task<IActionResult> GetUnitById(Guid id)
        {
            var unit = await _unitService.GetUnitByIdAsync(id);

            if (unit == null)
            {
                return NotFound();
            }

            var unitsEquipmentByProject = await _unitEquipmentService.GetAllUnitEquipmentsByProjectIdAsync(id);

            var exchangeRate = ExchangeRate.GetRate("usd");

            var unitPrice = unit.UnitPrices.Where(p => p.StageId.ToString() == unit.Model.Project.CurrentStageId.ToString()).FirstOrDefault();
            var priceList = unitPrice.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase)
                            ? Math.Round(unitPrice.Price * exchangeRate, 2)
                            : Math.Round(unitPrice.Price, 2);
            var minimumExpectedValue = unitPrice.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase)
                                        ? unit.UnitPrices.Max(x => x.Price * exchangeRate)
                                        : unit.UnitPrices.Max(x => x.Price);

            var viewModel = new UnitViewModel
            {
                Id = unit.Id,
                Name = unit.Model.Name,
                DisplayName = string.Format("{0} {1}", unit.Number, unit.Model.Name),
                Price = priceList,
                MinimumExpectedValue = minimumExpectedValue,
                Number = unit.Number,
                Level = unit.Level,
                GrossArea = unit.GrossArea,
                Status = unit.Status,
                AllowsCondoHotel = unit.Model.Project.AllowsCondoHotel,
                IsCondoHotel = unit.IsCondoHotel,
                ProjectAppreciation = unit.Model.Project.Appreciation,
                EquipmentPrice = Math.Round(unitsEquipmentByProject.Where(p => p.UnitId.Equals(unit.Id)).Sum(p => p.Cost), 4),
                PreOperatingCost = unit.Model.Project.CondoHotelPreOperatingCost.HasValue ? Math.Round(unit.Model.Project.CondoHotelPreOperatingCost.Value, 4) : 0m,
                Currency = "mxn"
            };

            return Ok(viewModel);
        }

        // POST api/units
        [HttpPost]
        public async Task<IActionResult> CreateUnit([FromBody] CreateUnitRequest resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Unit object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var unit = new Unit
            {
                ModelId = resource.ModelId,
                Number = resource.Number,
                Level = resource.Level,
                GrossArea = resource.GrossArea,
                BuiltUpArea = resource.BuiltUpArea,
                TerraceArea = resource.TerraceArea,
                Status = UnitStatus.ForSale
            };

            await _unitService.CreateUnitAsync(unit);

            return CreatedAtRoute("UnitById", new { id = resource.Id }, resource);
        }

        // PUT api/units/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit([FromBody] UpdateUnitRequest resource, Guid id)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Unit object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbUnit = await _unitService.GetUnitByIdAsync(id, false);
            if (dbUnit == null)
            {
                return NotFound();
            }

            var unit = new Unit
            {
                Number = resource.Number,
                // Level = resource.Level,
                // GrossArea = resource.GrossArea,
                // BuiltUpArea = resource.BuiltUpArea,
                // TerraceArea = resource.TerraceArea,
                Status = resource.Status,
                IsCondoHotel = resource.IsCondoHotel,
                // IsActive = resource.IsActive
            };

            await _unitService.UpdateUnitAsync(dbUnit, unit);
            await _unitService.UpdateUnitPriceAsync(id, resource.Price);

            return NoContent();
        }

        // DELETE api/units/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(Guid id)
        {
            var unit = await _unitService.GetUnitByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            await _unitService.DeleteUnitAsync(unit);

            return NoContent();
        }
    }
}