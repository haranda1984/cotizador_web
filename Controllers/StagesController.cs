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
    public class StagesController : ControllerBase
    {
        private readonly IStageService _stageService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public StagesController(IStageService stageService, IStringLocalizer<SharedResource> localizer)
        {
            _stageService = stageService;
            _localizer = localizer;
        }

        // GET api/stages/5
        [HttpGet("{id}", Name = "StageById")]
        public async Task<IActionResult> GetStageById(Guid id)
        {
            var stage = await _stageService.GetStageByIdAsync(id);

            if (stage == null)
            {
                return NotFound();
            }

            var viewModel = new StageViewModel
            {
                Id = stage.Id,
                Name = stage.Name,
                IsActive = stage.IsActive
            };

            return Ok(viewModel);
        }

        // POST api/stages
        [HttpPost]
        public async Task<IActionResult> CreateStage([FromBody] StageViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Stage object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var stage = new Stage
            {
                Id = resource.Id,
                Name = resource.Name,
                IsActive = resource.IsActive
            };

            await _stageService.CreateStageAsync(stage);

            return CreatedAtRoute("StageById", new { id = resource.Id }, resource);
        }

        // PUT api/stages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStage([FromBody] StageViewModel resource, Guid id)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Stage object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbStage = await _stageService.GetStageByIdAsync(id);
            if (dbStage == null)
            {
                return NotFound();
            }

            var stage = new Stage
            {
                Id = resource.Id,
                Name = resource.Name,
                IsActive = resource.IsActive
            };

            await _stageService.UpdateStageAsync(dbStage, stage);

            return NoContent();
        }

        // DELETE api/stages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStage(Guid id)
        {
            var stage = await _stageService.GetStageByIdAsync(id);
            if (stage == null)
            {
                return NotFound();
            }

            await _stageService.DeleteStageAsync(stage);

            return NoContent();
        }
    }
}