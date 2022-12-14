using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class RolesRepository : BaseRepository<Role>, IRolesRepository
    {
        public RolesRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await FindAll()
                        .ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(Guid roleId)
        {
            return await FindByCondition(role => role.Id.Equals(roleId))
                        .SingleOrDefaultAsync();
        }

        public async Task<Role> GetRoleByNameAsync(string name)
        {
            return await FindByCondition(role => role.Name.ToUpper() == name.ToUpper())
                        .AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task CreateRoleAsync(Role role)
        {
            role.Id = Guid.NewGuid();
            role.CreatedAt = role.ModifiedAt = DateTime.UtcNow;

            DetachLocal(role, p => p.Id.Equals(role.Id));
            Create(role);
            await SaveAsync();
        }

        public async Task UpdateRoleAsync(Role dbRole, Role role)
        {
            dbRole.Description = role.Description;
            dbRole.IsActive = role.IsActive;
            dbRole.ModifiedAt = DateTime.UtcNow;

            DetachLocal(dbRole, p => p.Id.Equals(dbRole.Id));
            Update(dbRole);
            await SaveAsync();
        }

        public async Task DeleteRoleAsync(Role role)
        {
            role.IsActive = false;
            role.ModifiedAt = DateTime.UtcNow;

            DetachLocal(role, p => p.Id.Equals(role.Id));
            // Delete(user);
            Update(role);
            await SaveAsync();
        }
    }
}