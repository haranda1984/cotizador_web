using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IRolesRepository _roleRepository;

        public UserService(IUsersRepository userRepository, IRolesRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return _userRepository.GetAllUsersAsync();
        }

        public Task<User> GetUserByIdAsync(Guid userId)
        {
            return _userRepository.GetUserByIdAsync(userId);
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            return _userRepository.GetUserByEmailAsync(email);
        }

        public Task CreateUserAsync(User user)
        {
            return _userRepository.CreateUserAsync(user);
        }

        public Task UpdateUserAsync(User dbUser, User user)
        {
            return _userRepository.UpdateUserAsync(dbUser, user);
        }

        public Task DeleteUserAsync(User user)
        {
            return _userRepository.DeleteUserAsync(user);
        }

        public Task AddUserToRoleAsync(User user, string roleName)
        {
            var role = _roleRepository.GetRoleByNameAsync(roleName).Result;
            var dbUser = _userRepository.GetUserByIdAsync(user.Id).Result;
            
            return _userRepository.AddUserToRoleAsync(dbUser, role);
        }

        public Task ClearUserRolesAsync(User user)
        {
            var dbUser = _userRepository.GetUserByIdAsync(user.Id).Result;

            return _userRepository.ClearUserRolesAsync(dbUser);
        }
    }
}