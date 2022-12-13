using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public interface IUsersRepository : IBaseRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> GetUserByEmailAsync(string email);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User dbUser, User user);
        Task UpdateUserPasswordAsync(User user, string password);
        Task DeleteUserAsync(User user);
        Task AddUserToRoleAsync(User user, Role role);
        Task ClearUserRolesAsync(User user);
        Task<User> AuthenticateUserAsync(string username, string password);
    }
}