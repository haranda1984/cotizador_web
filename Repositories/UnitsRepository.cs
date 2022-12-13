using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class UnitsRepository : BaseRepository<Unit>, IUnitsRepository
    {
        public UnitsRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task CreateUnitAsync(Unit unit)
        {
            unit.Id = Guid.NewGuid();
            unit.CreatedAt = unit.ModifiedAt = DateTime.UtcNow;
            unit.IsActive = true;

            DetachLocal(unit, p => p.Id.Equals(unit.Id));
            Create(unit);
            await SaveAsync();
        }

        public async Task DeleteUnitAsync(Unit unit)
        {
            unit.IsActive = false;
            unit.ModifiedAt = DateTime.UtcNow;

            DetachLocal(unit, p => p.Id.Equals(unit.Id));
            Update(unit);
            await SaveAsync();
        }

        public async Task<IEnumerable<Unit>> GetAllUnitsAsync()
        {
            return await FindAll()
                        .Include(p => p.Model).ThenInclude(p => p.Project).ThenInclude(p => p.CurrentStage).AsNoTracking()
                        .Include(p => p.UnitPrices).ThenInclude(p => p.Stage).AsNoTracking()
                        .ToListAsync();
        }

        public async Task<IEnumerable<Unit>> GetAllUnitsByProjectIdAsync(Guid projectId)
        {
            return await FindAll()
                        .Include(p => p.Model).ThenInclude(p => p.Project).ThenInclude(p => p.CurrentStage).AsNoTracking()
                        .Include(p => p.UnitPrices).ThenInclude(p => p.Stage).AsNoTracking()
                        .Where(p => p.Model.ProjectId.Equals(projectId))
                        .ToListAsync();
        }

        public async Task<Unit> GetUnitByIdAsync(Guid unitId, bool includeRelations = true)
        {
            if (includeRelations)
            {
                return await FindByCondition(unit => unit.Id.Equals(unitId))
                            .Include(p => p.Model).ThenInclude(p => p.Project).ThenInclude(p => p.CurrentStage).AsNoTracking()
                            .Include(p => p.UnitPrices).ThenInclude(p => p.Stage).AsNoTracking()
                            .SingleOrDefaultAsync();
            }
            else
            {
                return await FindByCondition(unit => unit.Id.Equals(unitId))
                            .SingleOrDefaultAsync();
            }
        }

        public async Task UpdateUnitAsync(Unit dbUnit, Unit unit)
        {
            dbUnit.Number = unit.Number;
            // dbUnit.Building = unit.Building;
            // dbUnit.Level = unit.Level;
            // dbUnit.GrossArea = unit.GrossArea;
            // dbUnit.BuiltUpArea = unit.BuiltUpArea;
            // dbUnit.TerraceArea = unit.TerraceArea;
            dbUnit.Status = unit.Status;
            dbUnit.IsCondoHotel = unit.IsCondoHotel;
            dbUnit.ModifiedAt = DateTime.UtcNow;

            DetachLocal(dbUnit, p => p.Id.Equals(dbUnit.Id));
            Update(dbUnit);
            await SaveAsync();
        }

        public async Task UpdateUnitPriceAsync(Guid unitId, decimal price)
        {
            var unit = await FindByCondition(unit => unit.Id.Equals(unitId))
                        .Include(p => p.Model).ThenInclude(p => p.Project).AsNoTracking()
                        .SingleAsync();

            var unitPrice = await _context.Set<UnitPrice>()
                                .Where(x => x.UnitId.Equals(unitId) && x.StageId.Equals(unit.Model.Project.CurrentStageId)).AsNoTracking()
                                .SingleAsync();

            unitPrice.Price = price;
            _context.Set<UnitPrice>().Update(unitPrice);

            await SaveAsync();
        }
    }
}