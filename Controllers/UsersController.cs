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
    [Authorize(Roles="super_admin,admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UsersController(IUserService userService, IStringLocalizer<SharedResource> localizer)
        {
            _userService = userService;
            _localizer = localizer;
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            var usersViewModel = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                Email = user.Profile?.Email,
                FirstName = user.Profile?.FirstName,
                LastName = user.Profile?.LastName,
                Position = user.Profile?.Position,
                Company = user.Profile?.Company,
                IsActive = user.IsActive,
                Roles = user.UserRoles?.Select(userRole => userRole.Role?.Name).ToList()
            });

            return Ok(usersViewModel);
        }

        // GET api/users/5
        [HttpGet("{id}", Name = "UserById")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Profile?.Email,
                FirstName = user.Profile?.FirstName,
                LastName = user.Profile?.LastName,
                Position = user.Profile?.Position,
                Company = user.Profile?.Company,
                IsActive = user.IsActive,
                Roles = user.UserRoles?.Select(userRole => userRole.Role?.Name).ToList()
            };

            return Ok(userViewModel);
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserViewModel resource)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["User object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var user = new User
            {
                Profile = new UserProfile
                {
                    FirstName = resource.FirstName,
                    LastName = resource.LastName,
                    Email = resource.Email,
                    Position = resource.Position,
                    Company = resource.Company,
                    Password = resource.Password
                },
                IsActive = true
            };

            await _userService.CreateUserAsync(user);

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Email = user.Profile?.Email,
                FirstName = user.Profile?.FirstName,
                LastName = user.Profile?.LastName,
                IsActive = user.IsActive
            };

            return Ok(userViewModel);

            // return CreatedAtRoute("UserById", new { id = resource.Id }, resource);
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel resource, Guid id)
        {
            if (resource == null)
            {
                return BadRequest(_localizer["User object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var dbUser = await _userService.GetUserByIdAsync(id);
            if (dbUser == null)
            {
                return NotFound();
            }

            var user = new User
            {
                Profile = new UserProfile
                {
                    FirstName = resource.FirstName,
                    LastName = resource.LastName,
                    Password = resource.Password,
                    Position = resource.Position,
                    Company = resource.Company
                },
                IsActive = resource.IsActive
            };

            await _userService.UpdateUserAsync(dbUser, user);

            return NoContent();
        }

        // DELETE api/users/5
        [Authorize(Roles="super_admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(user);

            return NoContent();
        }

        // GET api/users/5/role
        [HttpGet("{id}/role")]
        public async Task<IActionResult> GetUserRoles(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = user.UserRoles?.Select(userRole => new RoleViewModel { Id = userRole.RoleId, Name = userRole.Role?.Name }).ToList();

            return Ok(roles);
        }

        // POST api/users/5/roles
        [Authorize(Roles="super_admin")]
        [HttpPost("{id}/role")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] RoleViewModel[] resource, Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.ClearUserRolesAsync(user);
            foreach (var userRole in resource)
            {
                await _userService.AddUserToRoleAsync(user, userRole.Name);
            }

            return NoContent();
        }
    }
}