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
    public class TemporalitiesController : ControllerBase
    {
        private readonly ITemporalitiesService _temporalitiesService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TemporalitiesController(ITemporalitiesService temporalitiesService, IStringLocalizer<SharedResource> localizer)
        {
            _temporalitiesService = temporalitiesService;
            _localizer = localizer;
        }

        // GET api/Temporalities
        [HttpGet]
        public async Task<IActionResult> GetAllTemporalitiesAsync()
        {
            var temporalities = await _temporalitiesService.GetAllTemporalitiesAsync();

            if (temporalities == null)
            {
                return NotFound();
            }

            var viewModel = temporalities.Select(temporality => new TemporalityViewModel
            {
                ProjectId = temporality.ProjectId,
                RateId = temporality.RateId,
                Month = temporality.Month,
                OccupationInDays = temporality.OccupationInDays,
            });

            return Ok(viewModel);
        }


        // GET api/Temporalities/5
        [HttpGet("{id}", Name = "TemporalitiesByProjectId")]
        public async Task<IActionResult> GetAllTemporalitiesByProjectIdAsync(Guid projectId)
        {
            var temporalities = await _temporalitiesService.GetAllTemporalitiesByProjectIdAsync(projectId);

            if (temporalities == null)
            {
                return NotFound();
            }

            var viewModel = temporalities.Select(temporality => new TemporalityViewModel
            {
                ProjectId = temporality.ProjectId,
                RateId = temporality.RateId,
                Month = temporality.Month,
                OccupationInDays = temporality.OccupationInDays,
            });

            return Ok(viewModel);
        }

        // GET api/Temporalities/{name}
        [HttpGet("{name}", Name = "TemporalityById")]
        public async Task<IActionResult> GetTemporalityByIdsAsync(Guid projectId, Guid ratesId)
        {
            var temporality = await _temporalitiesService.GetTemporalityByIdsAsync(projectId, ratesId);

            if (temporality == null)
            {
                return NotFound();
            }

            var viewModel = new TemporalityViewModel
            {
                ProjectId = temporality.ProjectId,
                RateId = temporality.RateId,
                Month = temporality.Month,
                OccupationInDays = temporality.OccupationInDays,
            };

            return Ok(viewModel);
        }

        // POST api/Temporalities
        [HttpPost]
        public async Task<IActionResult> CreateTemporality([FromBody] TemporalityViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Temporality object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var temporality = new Temporality
            {
                ProjectId = resource.ProjectId,
                RateId = resource.RateId,
                Month = resource.Month,
                OccupationInDays = resource.OccupationInDays,
            };

            await _temporalitiesService.CreateTemporalityAsync(temporality);

            return CreatedAtRoute("TemporalityById", new { ProjectId = resource.ProjectId, RateId = resource.RateId }, resource);
        }

        // PUT api/Temporalities/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemporality([FromBody] TemporalityViewModel resource, Guid projectId, Guid ratesId)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Temporality object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbTemporality = await _temporalitiesService.GetTemporalityByIdsAsync(projectId, ratesId);
            if (dbTemporality == null)
            {
                return NotFound();
            }

            var temporality = new Temporality
            {
                ProjectId = resource.ProjectId,
                RateId = resource.RateId,
                Month = resource.Month,
                OccupationInDays = resource.OccupationInDays,
            };

            await _temporalitiesService.UpdateTemporalityAsync(dbTemporality, temporality);

            return NoContent();
        }

        // DELETE api/Temporalities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemporality(Guid projectId, Guid ratesId)
        {
            var temporality = await _temporalitiesService.GetTemporalityByIdsAsync(projectId, ratesId);
            if (temporality == null)
            {
                return NotFound();
            }

            await _temporalitiesService.DeleteTemporalityAsync(temporality);

            return NoContent();
        }
    }
}