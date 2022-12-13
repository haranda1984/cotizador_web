using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsRepository _projectRepository;

        public ProjectsService(IProjectsRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public Task CreateProjectAsync(Project project)
        {
            return _projectRepository.CreateProjectAsync(project);
        }

        public Task DeleteProjectAsync(Project project)
        {
            return _projectRepository.DeleteProjectAsync(project);
        }

        public Task UpdateProjectAsync(Project dbProject, Project project)
        {
            return _projectRepository.UpdateProjectAsync(dbProject, project);
        }                

        public Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return _projectRepository.GetAllProjectsAsync();
        }

        public Task<Project> GetProjectByIdAsync(Guid projectId, bool includeRelations = true)
        {
            return _projectRepository.GetProjectByIdAsync(projectId, includeRelations);
        }

        public Task<Project> GetProjectByNameAsync(string name)
        {
            return _projectRepository.GetProjectByNameAsync(name);
        }
    }
}