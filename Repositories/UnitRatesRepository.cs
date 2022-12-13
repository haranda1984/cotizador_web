using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class UnitRatesRepository : BaseRepository<UnitRate>, IUnitRatesRepository
    {
        public UnitRatesRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<UnitRate>> GetAllUnitRatesAsync()
        {
            return await FindAll()
                        .Include(s => s.Unit).AsNoTracking()
                        .ToListAsync();
        }

        public async Task<UnitRate> GetUnitRateByIdsAsync(Guid unitId, Guid rateId)
        {
            return await FindByCondition(x => x.UnitId.Equals(unitId))
                        .Include(s => s.Unit).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<UnitRate>> GetAllUnitRatesByUnitIdAsync(Guid unitId)
        {
            return await FindByCondition(x => x.UnitId.Equals(unitId))
                        .Include(s => s.Unit).AsNoTracking()
                        .ToListAsync();
        }


        public async Task CreateUnitRateAsync(UnitRate unitRate)
        {            
            DetachLocal(unitRate, u => u.UnitId.Equals(unitRate.UnitId));
            Create(unitRate);
            await SaveAsync();
        }

        public async Task UpdateUnitRateAsync(UnitRate dbUnitRate, UnitRate unitRate)
        {
            dbUnitRate.CostPerNight = unitRate.CostPerNight;
            dbUnitRate.BuiltUpAreaCost = unitRate.BuiltUpAreaCost;
            dbUnitRate.TerraceAreaCost = unitRate.TerraceAreaCost;

            DetachLocal(dbUnitRate, u => u.UnitId.Equals(dbUnitRate.UnitId));
            Update(dbUnitRate);
            await SaveAsync();
        }

        public Task DeleteUnitRateAsync(UnitRate unitRate)
        {
            throw new NotImplementedException();
        }
    }
}