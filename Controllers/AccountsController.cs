using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using HeiLiving.Quotes.Api.Services;
using HeiLiving.Quotes.Api.Models;

namespace HeiLiving.Quotes.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private IAccountsService _accountService;
        private IUserService _userService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccountsController(IAccountsService accountService, IUserService userService, IStringLocalizer<SharedResource> localizer, IConfiguration config)
        {
            _accountService = accountService;
            _userService = userService;
            _localizer = localizer;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] UserAuthViewModel user)
        {
            if (user == null)
            {
                return BadRequest(_localizer["User object is null"].Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(_localizer["Invalid model object"].Value);
            }

            var token = _accountService.Authenticate(user.Username, user.Password).Result;

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = _localizer["Username or password is incorrect"].Value });
            }

            return Ok(token);
        }

        [HttpGet("me", Name = "UserIdentity")]
        public IActionResult GetUserIdentity()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return BadRequest(_localizer["There is no claims identity"].Value);
            }

            var userId = identity.FindFirst("id").Value;

            var user = _userService.GetUserByIdAsync(new Guid(userId)).Result;

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.Profile.Id,
                user.Profile.FirstName,
                user.Profile.LastName,
                user.Profile.Email
            });
        }

        [HttpPost("me/changePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return BadRequest(_localizer["There is no claims identity"].Value);
            }
            var userId = identity.FindFirst("id").Value;

            var user = await _userService.GetUserByIdAsync(new Guid(userId));
            await _accountService.UpdateUserPassword(user, input.NewPassword);

            return NoContent();
        }

        [Authorize(Roles = "super_admin,admin")]
        [HttpPost("{id}/changePassword")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] ChangePasswordViewModel input, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _accountService.UpdateUserPassword(user, input.NewPassword);

            return NoContent();
        }
    }
}