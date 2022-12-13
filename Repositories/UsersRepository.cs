using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class UsersRepository : BaseRepository<User>, IUsersRepository
    {
        private PasswordHasher _hasher { get; }        

        public UsersRepository(ApplicationDbContext context)
            : base(context)
        {
            _hasher = new PasswordHasher(new OptionsWrapper<HashingOptions>(new HashingOptions() { Iterations = 1000 }));
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await FindAll()
                        .Include(p => p.Profile).AsNoTracking()
                        .Include(p => p.UserRoles).ThenInclude(p => p.Role).AsNoTracking()
                        .ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await FindByCondition(user => user.Id.Equals(userId))
                        .Include(p => p.Profile).AsNoTracking()
                        .Include(p => p.UserRoles).ThenInclude(p => p.Role).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await FindByCondition(user => user.Profile.Email.ToUpper() == email.ToUpper())
                        .Include(p => p.Profile).AsNoTracking()
                        .Include(p => p.UserRoles).ThenInclude(p => p.Role).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            user.Id = Guid.NewGuid();
            user.CreatedAt = user.ModifiedAt = DateTime.UtcNow;

            user.Profile.Id = Guid.NewGuid();
            user.Profile.Password = !string.IsNullOrEmpty(user.Profile.Password) ? _hasher.Hash(user.Profile.Password) : string.Empty;

            DetachLocal(user, p => p.Id.Equals(user.Id));
            Create(user);
            await SaveAsync();
        }

        public async Task UpdateUserAsync(User dbUser, User user)
        {
            dbUser.IsActive = user.IsActive;
            dbUser.ModifiedAt = DateTime.UtcNow;

            dbUser.Profile.FirstName = user.Profile.FirstName;
            dbUser.Profile.LastName = user.Profile.LastName;
            dbUser.Profile.Password = !string.IsNullOrEmpty(user.Profile.Password) ? _hasher.Hash(user.Profile.Password) : dbUser.Profile.Password;
            dbUser.Profile.Position = user.Profile.Position;
            dbUser.Profile.Company = user.Profile.Company;

            DetachLocal(dbUser, p => p.Id.Equals(dbUser.Id));
            Update(dbUser);
            await SaveAsync();
        }

        public async Task UpdateUserPasswordAsync(User user, string newPassword)
        {
            user.Profile.Password = !string.IsNullOrEmpty(newPassword) ? _hasher.Hash(newPassword) : string.Empty;
            user.ModifiedAt = DateTime.UtcNow;

            DetachLocal(user, p => p.Id.Equals(user.Id));
            Update(user);

            await SaveAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            user.IsActive = false;
            user.ModifiedAt = DateTime.UtcNow;

            DetachLocal(user, p => p.Id.Equals(user.Id));
            // Delete(user);
            Update(user);
            await SaveAsync();
        }

        public async Task AddUserToRoleAsync(User user, Role role)
        {
            var userRole = new UserRole {
                UserId = user.Id,
                RoleId = role.Id
            };

            var localUser = _context.Set<User>().Local.FirstOrDefault(p => p.Id.Equals(user.Id));
            if (localUser != null)
            {
                _context.Entry(localUser).State = EntityState.Detached;
            }

            var localRole = _context.Set<Role>().Local.FirstOrDefault(p => p.Id.Equals(role.Id));
            if (localRole != null)
            {
                _context.Entry(localRole).State = EntityState.Detached;
            }

            _context.Set<UserRole>().Add(userRole);

            await SaveAsync();
        }

        public async Task ClearUserRolesAsync(User user) {  
            var localUser = _context.Set<User>().Local.FirstOrDefault(p => p.Id.Equals(user.Id));
            if (localUser != null)
            {
                _context.Entry(localUser).State = EntityState.Detached;
            }

            foreach (var userRole in user.UserRoles) {
                var localRole = _context.Set<Role>().Local.FirstOrDefault(p => p.Id.Equals(userRole.RoleId));
                if (localRole != null)
                {
                    _context.Entry(localRole).State = EntityState.Detached;
                }

                var localUserRole = _context.Set<UserRole>()
                                        .Local
                                        .FirstOrDefault(p => p.UserId.Equals(user.Id) && p.RoleId.Equals(userRole.RoleId));
                if (localUserRole != null)
                {
                    _context.Entry(localUserRole).State = EntityState.Detached;
                }

                _context.Set<UserRole>().Remove(userRole);
            }
            
            await SaveAsync();
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var userdb = FindByCondition(user => user.IsActive && user.Profile.Email.Equals(email))
                        .Include(p => p.Profile).AsNoTracking()
                        .Include(p => p.UserRoles).ThenInclude(p => p.Role).AsNoTracking()
                        .SingleOrDefaultAsync()
                        .Result;

            if (userdb == null || userdb.Profile == null)
            {
                return await Task.FromResult<User>(null);
            }

            var result = _hasher.Check(userdb.Profile.Password, password);

            if (result.Verified)
            {
                userdb.FailedLoginAttempts = 0;
                userdb.SuccessfulLoginAttempts++;
                userdb.LastLoggedIn = DateTime.UtcNow;

                DetachLocal(userdb, p => p.Id.Equals(userdb.Id));
                Update(userdb);
                await SaveAsync();

                return await Task.FromResult<User>(userdb);
            }

            userdb.FailedLoginAttempts++;

            DetachLocal(userdb, p => p.Id.Equals(userdb.Id));
            Update(userdb);
            await SaveAsync();

            return await Task.FromResult<User>(null);
        }
    }
}