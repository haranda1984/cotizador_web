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

namespace HeiLiving.Quotes.Api.Controllers
{
    [Authorize(Roles="super_admin,admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RolesController(IRoleService roleService, IStringLocalizer<SharedResource> localizer)
        {
            _roleService = roleService;
            _localizer = localizer;
        }

        // GET api/roles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();

            var viewModel = roles.Select(role => new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            });

            return Ok(viewModel);
        }

        // GET api/roles/5
        [HttpGet("{id}", Name = "RoleById")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };

            return Ok(viewModel);
        }

        // POST api/roles
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Role object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var role = new Role
            {
                Name = resource.Name,
                Description = resource.Description,
                IsActive = true
            };

            await _roleService.CreateRoleAsync(role);

            return CreatedAtRoute("RoleById", new { id = resource.Id }, resource);
        }

        // PUT api/roles/5
        [Authorize(Roles="super_admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleViewModel resource, Guid id)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["Role object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbRole = await _roleService.GetRoleByIdAsync(id);
            if (dbRole == null)
            {
                return NotFound();
            }

            var role = new Role
            {
                Name = resource.Name,
                Description = resource.Description
            };

            await _roleService.UpdateRoleAsync(dbRole, role);

            return NoContent();
        }

        // DELETE api/roles/5
        [Authorize(Roles="super_admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            await _roleService.DeleteRoleAsync(role);

            return NoContent();
        }
    }
}