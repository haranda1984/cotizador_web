using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRolesRepository _roleRepository;

        public RoleService(IRolesRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return _roleRepository.GetAllRolesAsync();
        }

        public Task<Role> GetRoleByIdAsync(Guid roleId)
        {
            return _roleRepository.GetRoleByIdAsync(roleId);
        }

        public Task<Role> GetRoleByNameAsync(string name)
        {
            return _roleRepository.GetRoleByNameAsync(name);
        }

        public Task CreateRoleAsync(Role role)
        {
            return _roleRepository.CreateRoleAsync(role);
        }

        public Task UpdateRoleAsync(Role dbRole, Role role)
        {
            return _roleRepository.UpdateRoleAsync(dbRole, role);
        }

        public Task DeleteRoleAsync(Role role)
        {
            return _roleRepository.DeleteRoleAsync(role);
        }
    }
}