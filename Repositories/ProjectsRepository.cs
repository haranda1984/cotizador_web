using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class ProjectsRepository : BaseRepository<Project>, IProjectsRepository
    {
        public ProjectsRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task CreateProjectAsync(Project project)
        {
            project.Id = Guid.NewGuid();
            project.CreatedAt = project.ModifiedAt = DateTime.UtcNow;

            DetachLocal(project, p => p.Id.Equals(project.Id));
            Create(project);
            await SaveAsync();
        }

        public async Task DeleteProjectAsync(Project project)
        {
            project.IsActive = false;
            project.ModifiedAt = DateTime.UtcNow;

            DetachLocal(project, p => p.Id.Equals(project.Id));
            Update(project);
            await SaveAsync();
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await FindAll()
                        .Include(p => p.CurrentStage).AsNoTracking()
                        .Include(p => p.ProjectStages).ThenInclude(p => p.Stage).AsNoTracking()
                        .ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(Guid projectId, bool includeRelations = true)
        {
            if (includeRelations)
            {
                return await FindByCondition(project => project.Id.Equals(projectId))
                            .Include(p => p.CurrentStage).AsNoTracking()
                            .Include(p => p.ProjectStages).ThenInclude(p => p.Stage).AsNoTracking()
                            .SingleOrDefaultAsync();
            }
            else
            {
                return await FindByCondition(project => project.Id.Equals(projectId))
                        .SingleOrDefaultAsync();
            }
        }

        public async Task<Project> GetProjectByNameAsync(string name)
        {
            return await FindByCondition(project => project.Name.ToUpper() == name.ToUpper())
                        .Include(p => p.CurrentStage).AsNoTracking()
                        .Include(p => p.ProjectStages).ThenInclude(p => p.Stage).AsNoTracking()
                        .SingleOrDefaultAsync();
        }

        public async Task UpdateProjectAsync(Project dbProject, Project project)
        {
            dbProject.Name = project.Name;
            dbProject.DisplayName = project.DisplayName;
            dbProject.Description = project.Description;
            dbProject.PhotoUrl = project.PhotoUrl;
            dbProject.ThemeColor = project.ThemeColor;
            dbProject.Location = project.Location;
            dbProject.DeliveryDate = project.DeliveryDate;
            dbProject.ModifiedAt = DateTime.UtcNow;
            dbProject.AllowsCondoHotel = project.AllowsCondoHotel;
            dbProject.MinimumCondoHotelUnits = project.MinimumCondoHotelUnits;
            dbProject.CurrentStageId = project.CurrentStageId;
            dbProject.CapRate = project.CapRate;

            DetachLocal(dbProject, p => p.Id.Equals(dbProject.Id));
            Update(dbProject);

            await SaveAsync();
        }
    }
}