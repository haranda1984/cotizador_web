using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public interface IStagesRepository : IBaseRepository<Stage>
    {
        Task<IEnumerable<Stage>> GetAllStagesAsync();
        Task<Stage> GetStageByIdAsync(Guid stageId);
        Task<Stage> GetStageByNameAsync(string name);
        Task<IEnumerable<Stage>> GetAllStagesByProjectIdAsync(Guid projectId);
        Task CreateStageAsync(Stage stage);
        Task UpdateStageAsync(Stage dbStage, Stage stage);
        Task DeleteStageAsync(Stage stage);
    }
}

