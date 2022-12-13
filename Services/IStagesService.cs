using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Services
{
    public interface IStageService
    {
        Task<IEnumerable<Stage>> GetAllStagesAsync();
        Task<IEnumerable<Stage>> GetAllStagesByProjectIdAsync(Guid projectId);
        Task<Stage> GetStageByIdAsync(Guid stageId);
        Task<Stage> GetStageByNameAsync(string name);
        Task CreateStageAsync(Stage stage);
        Task UpdateStageAsync(Stage dbStage, Stage stage);
        Task DeleteStageAsync(Stage stage);
    }
}