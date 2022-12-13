using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class RatesRepository : BaseRepository<Rate>, IRatesRepository
    {
        public RatesRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Rate>> GetAllRatesAsync()
        {
            return await FindAll()
                        .ToListAsync();
        }

        public async Task<Rate> GetRateByIdAsync(Guid RateId)
        {
            return await FindByCondition(r => r.Id.Equals(RateId))
                        .SingleOrDefaultAsync();
        }

        public async Task<Rate> GetRateByNameAsync(string name)
        {
            return await FindByCondition(r => r.Name.ToUpper() == name.ToUpper()).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task CreateRateAsync(Rate Rate)
        {
            Rate.Id = Guid.NewGuid();

            DetachLocal(Rate, p => p.Id.Equals(Rate.Id));
            Create(Rate);
            await SaveAsync();
        }

        public async Task UpdateRateAsync(Rate dbRate, Rate rates)
        {
            dbRate.Name = rates.Name;
            dbRate.Description = rates.Description;

            DetachLocal(dbRate, p => p.Id.Equals(dbRate.Id));
            Update(dbRate);
            await SaveAsync();
        }

        public Task DeleteRateAsync(Rate Rate)
        {
            throw new NotImplementedException();
        }
    }
}