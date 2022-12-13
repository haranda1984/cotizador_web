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
    public class UnitEquipmentsController : ControllerBase
    {
        private readonly IUnitEquipmentsService _UnitEquipmentsService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UnitEquipmentsController(IUnitEquipmentsService UnitEquipmentsService, IStringLocalizer<SharedResource> localizer)
        {
            _UnitEquipmentsService = UnitEquipmentsService;
            _localizer = localizer;
        }

        // GET api/UnitEquipments
        [HttpGet]
        public async Task<IActionResult> GetAllUnitEquipmentsAsync()
        {
            var unitEquipments = await _UnitEquipmentsService.GetAllUnitEquipmentsAsync();

            if (unitEquipments == null)
            {
                return NotFound();
            }

            var viewModel = unitEquipments.Select(unitEquipments => new UnitEquipmentViewModel
            {
                Id = unitEquipments.Id,
                UnitId = unitEquipments.UnitId,
                Description = unitEquipments.Description,
                Cost = unitEquipments.Cost,
            });

            return Ok(viewModel);
        }

        // GET api/UnitEquipments/5
        [HttpGet("{id}", Name = "UnitEquipmentsById")]
        public async Task<IActionResult> GetUnitEquipmentByIdAsync(Guid unitEquipmentsId)
        {
            var unitEquipment = await _UnitEquipmentsService.GetUnitEquipmentByIdAsync(unitEquipmentsId);

            if (unitEquipment == null)
            {
                return NotFound();
            }

            var viewModel = new UnitEquipmentViewModel
            {
                Id = unitEquipment.Id,
                UnitId = unitEquipment.UnitId,
                Description = unitEquipment.Description,
                Cost = unitEquipment.Cost,
            };

            return Ok(viewModel);
        }

        // GET api/UnitEquipments/{name}
        [HttpGet("{description}", Name = "UnitEquipmentsByName")]
        public async Task<IActionResult> GetUnitEquipmentsByDescriptionAsync(string description)
        {
            var unitEquipments = await _UnitEquipmentsService.GetUnitEquipmentsByDescriptionAsync(description);

            if (unitEquipments == null)
            {
                return NotFound();
            }

            var viewModel = unitEquipments.Select(unitEquipment => new UnitEquipmentViewModel
            {
                Id = unitEquipment.Id,
                UnitId = unitEquipment.UnitId,
                Description = unitEquipment.Description,
                Cost = unitEquipment.Cost,
            });

            return Ok(viewModel);
        }

        // GET api/UnitEquipments/Project/5
        [HttpGet("Project/{projectId}", Name = "UnitEquipmentsByProjectId")]
        public async Task<IActionResult> GetAllUnitEquipmentsByProjectIdAsync(Guid projectId)
        {
            var unitEquipments = await _UnitEquipmentsService.GetAllUnitEquipmentsByProjectIdAsync(projectId);

            if (unitEquipments == null)
            {
                return NotFound();
            }

            var viewModel = unitEquipments.Select(unitEquipment => new UnitEquipmentViewModel
            {
                Id = unitEquipment.Id,
                UnitId = unitEquipment.UnitId,
                Description = unitEquipment.Description,
                Cost = unitEquipment.Cost,
            });

            return Ok(viewModel);
        }


        // POST api/UnitEquipments
        [HttpPost]
        public async Task<IActionResult> CreateUnitEquipment([FromBody] UnitEquipmentViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["UnitEquipments object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var unitEquipment = new UnitEquipment
            {
                Id = resource.Id,
                UnitId = resource.UnitId,
                Description = resource.Description,
                Cost = resource.Cost,
            };

            await _UnitEquipmentsService.CreateUnitEquipmentAsync(unitEquipment);

            return CreatedAtRoute("UnitEquipmentById", new { id = resource.Id }, resource);
        }

        // PUT api/UnitEquipments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnitEquipment([FromBody] UnitEquipmentViewModel resource, Guid id)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["UnitEquipments object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbUnitEquipment = await _UnitEquipmentsService.GetUnitEquipmentByIdAsync(id);
            if (dbUnitEquipment == null)
            {
                return NotFound();
            }

            var unitEquipment = new UnitEquipment
            {
                Id = resource.Id,
                UnitId = resource.UnitId,
                Description = resource.Description,
                Cost = resource.Cost,
            };

            await _UnitEquipmentsService.UpdateUnitEquipmentAsync(dbUnitEquipment, unitEquipment);

            return NoContent();
        }

        // DELETE api/UnitEquipments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnitEquipment(Guid id)
        {
            var unitEquipments = await _UnitEquipmentsService.GetUnitEquipmentByIdAsync(id);
            if (unitEquipments == null)
            {
                return NotFound();
            }

            await _UnitEquipmentsService.DeleteUnitEquipmentAsync(unitEquipments);

            return NoContent();
        }
    }
}