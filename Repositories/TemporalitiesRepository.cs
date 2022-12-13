using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class TemporalitiesRepository : BaseRepository<Temporality>, ITemporalitiesRepository
    {
        public TemporalitiesRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Temporality>> GetAllTemporalitiesAsync()
        {
            return await FindAll()
                        .Include(s => s.Project).AsNoTracking()
                        .Include(s => s.Rate).AsNoTracking()
                        .ToListAsync();
        }


        public async Task<IEnumerable<Temporality>> GetAllTemporalitiesByProjectIdAsync(Guid projectId)
        {
            return await FindByCondition(x => x.ProjectId.Equals(projectId))
                            .Include(s => s.Project).AsNoTracking()
                            .Include(s => s.Rate).AsNoTracking()
                            .ToListAsync();
        }


        public async Task<Temporality> GetTemporalityByIdsAsync(Guid projectId, Guid ratesId)
        {
            return await FindByCondition(Temporalities => Temporalities.ProjectId.Equals(projectId) && Temporalities.ProjectId.Equals(ratesId))
                        .Include(s => s.Project).AsNoTracking()
                        .Include(s => s.Rate).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task CreateTemporalityAsync(Temporality temporality)
        {
            DetachLocal(temporality, p => p.ProjectId.Equals(temporality.ProjectId) && p.RateId.Equals(temporality.RateId));
            Create(temporality);
            await SaveAsync();
        }

        public async Task UpdateTemporalityAsync(Temporality dbTemporality, Temporality temporality)
        {
            dbTemporality.Month = temporality.Month;
            dbTemporality.OccupationInDays = temporality.OccupationInDays;

            DetachLocal(dbTemporality, p => p.ProjectId.Equals(dbTemporality.ProjectId) && p.RateId.Equals(dbTemporality.RateId));
            Update(dbTemporality);
            await SaveAsync();
        }

        public Task DeleteTemporalityAsync(Temporality temporality)
        {
            throw new NotImplementedException();
        }
    }
}