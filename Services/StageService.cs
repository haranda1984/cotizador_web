using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class StageService : IStageService
    {
        private readonly IStagesRepository _stageRepository;

        public StageService(IStagesRepository stageRepository)
        {
            _stageRepository = stageRepository;
        }

        public Task<IEnumerable<Stage>> GetAllStagesAsync()
        {
            return _stageRepository.GetAllStagesAsync();
        }

        public Task<IEnumerable<Stage>> GetAllStagesByProjectIdAsync(Guid projectId)
        {
            return _stageRepository.GetAllStagesByProjectIdAsync(projectId);
        }

        public Task<Stage> GetStageByIdAsync(Guid stageId)
        {
            return _stageRepository.GetStageByIdAsync(stageId);
        }

        public Task<Stage> GetStageByNameAsync(string name)
        {
            return _stageRepository.GetStageByNameAsync(name);
        }

        public Task CreateStageAsync(Stage stage)
        {
            return _stageRepository.CreateStageAsync(stage);
        }

        public Task UpdateStageAsync(Stage dbstage, Stage stage)
        {
            return _stageRepository.UpdateStageAsync(dbstage, stage);
        }
        public Task DeleteStageAsync(Stage stage)
        {
            return _stageRepository.DeleteStageAsync(stage);
        }
    }
}