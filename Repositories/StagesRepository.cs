using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class StagesRepository : BaseRepository<Stage>, IStagesRepository
    {
        public StagesRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Stage>> GetAllStagesAsync()
        {
            return await FindAll()
                        .Include(s => s.ProjectStages).AsNoTracking()
                        .Include(p => p.UnitPrices).AsNoTracking()
                        .ToListAsync();
        }


        public async Task<IEnumerable<Stage>> GetAllStagesByProjectIdAsync(Guid projectId)
        {
            var results = await _context.Projects.Where(q => q.Id.Equals(projectId))
                        .Include(q => q.ProjectStages).ThenInclude(q => q.Stage).AsNoTracking()
                        .Select(q => q.ProjectStages)
                        .SingleOrDefaultAsync();

            return results.Select(x => x.Stage);
        }


        public async Task<Stage> GetStageByIdAsync(Guid StageId)
        {
            return await FindByCondition(Stage => Stage.Id.Equals(StageId))
                        .Include(s => s.ProjectStages).AsNoTracking()
                        .Include(p => p.UnitPrices).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task<Stage> GetStageByNameAsync(string name)
        {
            return await FindByCondition(Stage => Stage.Name.ToUpper() == name.ToUpper()).AsNoTracking()
                        .Include(s => s.ProjectStages).AsNoTracking()
                        .Include(p => p.UnitPrices).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task CreateStageAsync(Stage Stage)
        {
            Stage.Id = Guid.NewGuid();

            DetachLocal(Stage, p => p.Id.Equals(Stage.Id));
            Create(Stage);
            await SaveAsync();
        }

        public async Task UpdateStageAsync(Stage dbStage, Stage Stage)
        {
            dbStage.Name = Stage.Name;
            dbStage.ProjectStages = Stage.ProjectStages;
            dbStage.IsActive = Stage.IsActive;
            dbStage.UnitPrices = Stage.UnitPrices;

            DetachLocal(dbStage, p => p.Id.Equals(dbStage.Id));
            Update(dbStage);
            await SaveAsync();
        }

        public async Task DeleteStageAsync(Stage Stage)
        {
            Stage.IsActive = false;

            DetachLocal(Stage, p => p.Id.Equals(Stage.Id));
            Update(Stage);
            await SaveAsync();
        }
    }
}