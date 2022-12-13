using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class TemporalitiesService : ITemporalitiesService
    {
        private readonly ITemporalitiesRepository _temporalitiesRepository;

        public TemporalitiesService(ITemporalitiesRepository stageRepository)
        {
            _temporalitiesRepository = stageRepository;
        }

        public Task<IEnumerable<Temporality>> GetAllTemporalitiesAsync()
        {
            return _temporalitiesRepository.GetAllTemporalitiesAsync();
        }

        public Task<IEnumerable<Temporality>> GetAllTemporalitiesByProjectIdAsync(Guid projectId)
        {
            return _temporalitiesRepository.GetAllTemporalitiesByProjectIdAsync(projectId);
        }
        
        public Task CreateTemporalityAsync(Temporality stage)
        {
            return _temporalitiesRepository.CreateTemporalityAsync(stage);
        }

        public Task UpdateTemporalityAsync(Temporality dbstage, Temporality stage)
        {
            return _temporalitiesRepository.UpdateTemporalityAsync(dbstage, stage);
        }
        public Task DeleteTemporalityAsync(Temporality stage)
        {
            return _temporalitiesRepository.DeleteTemporalityAsync(stage);
        }

        public Task<Temporality> GetTemporalityByIdsAsync(Guid projectId, Guid ratesId)
        {
            return _temporalitiesRepository.GetTemporalityByIdsAsync(projectId, ratesId);
        }
    }
}