using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Text.Json;
using HeiLiving.Quotes.Api.Auth;
using HeiLiving.Quotes.Api.Models;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;
using HeiLiving.Quotes.Api.Helpers;

namespace HeiLiving.Quotes.Api.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IAuthorizationCodeFactory _authCodeFactory;
        private readonly IConfiguration _config;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccountsService(
            IUsersRepository userRepository,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IAuthorizationCodeFactory authCodeFactory,
            IConfiguration config,
            IStringLocalizer<SharedResource> localizer)
        {
            _userRepository = userRepository;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _authCodeFactory = authCodeFactory;
            _config = config;
            _localizer = localizer;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var user = _userRepository.AuthenticateUserAsync(username, password).Result;

            // return null if user not found
            if (user == null)
            {
                return null;
            }

            var roles = user.UserRoles?.Select(x => x.Role.Name).ToArray();
            var identity = _jwtFactory.GenerateClaimsIdentity(user.Profile.Email, user.Id, roles);
            return await Tokens.GenerateJwt(identity, _jwtFactory, user.Profile.Email, _jwtOptions, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task RegisterUser(User user)
        {
            await _userRepository.CreateUserAsync(user);
        }

        public async Task UpdateUserPassword(User user, string newPassword)
        {
            // return null if user not found
            if (user == null)
            {
                return;
            }

            await _userRepository.UpdateUserPasswordAsync(user, newPassword);
        }
    }
}