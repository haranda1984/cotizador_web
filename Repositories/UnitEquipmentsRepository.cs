using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class UnitEquipmentsRepository : BaseRepository<UnitEquipment>, IUnitEquipmentsRepository
    {
        public UnitEquipmentsRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<UnitEquipment>> GetAllUnitEquipmentsAsync()
        {
            return await FindAll()
                        .Include(p => p.Unit).AsNoTracking()
                        .ToListAsync();
        }

        public async Task<IEnumerable<UnitEquipment>> GetAllUnitEquipmentsByProjectIdAsync(Guid projectId)
        {
            return await FindByCondition(u => u.Unit.Model.ProjectId.Equals(projectId))
                        .Include(p => p.Unit).ThenInclude(m => m.Model).AsNoTracking()
                        .ToListAsync();
        }

        public async Task<IEnumerable<UnitEquipment>> GetUnitEquipmentsByDescriptionAsync(string description)
        {
            return await FindByCondition(u => u.Description.Contains(description))
                        .Include(p => p.Unit).AsNoTracking()
                        .ToListAsync();
        }

        public async Task<UnitEquipment> GetUnitEquipmentByIdAsync(Guid unitEquipmentId)
        {
            return await FindByCondition(unitEquipment => unitEquipment.Id.Equals(unitEquipmentId))
                        .Include(p => p.Unit).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task CreateUnitEquipmentAsync(UnitEquipment unitEquipment)
        {
            unitEquipment.Id = Guid.NewGuid();
            
            DetachLocal(unitEquipment, u => u.Id.Equals(unitEquipment.Id));
            Update(unitEquipment);
            await SaveAsync();
        }

        public async Task UpdateUnitEquipmentAsync(UnitEquipment dbUnitEquipment, UnitEquipment unitEquipment)
        {
            dbUnitEquipment.Cost = unitEquipment.Cost;

            DetachLocal(dbUnitEquipment, u => u.Id.Equals(dbUnitEquipment.Id));
            Update(dbUnitEquipment);
            await SaveAsync();
        }
        public Task DeleteUnitEquipmentAsync(UnitEquipment unitEquipment)
        {
            throw new NotImplementedException();
        }
    }
}