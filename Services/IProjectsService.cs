using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Services
{
    public interface IProjectsService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project> GetProjectByIdAsync(Guid projectId, bool includeRelations = true);
        Task<Project> GetProjectByNameAsync(string name);
        Task CreateProjectAsync(Project project);
        Task UpdateProjectAsync(Project dbProject, Project project);
        Task DeleteProjectAsync(Project project);
    }
}