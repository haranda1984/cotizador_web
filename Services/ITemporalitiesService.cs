using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Services
{
    public interface ITemporalitiesService
    {
        Task<IEnumerable<Temporality>> GetAllTemporalitiesAsync();        
        Task<IEnumerable<Temporality>> GetAllTemporalitiesByProjectIdAsync(Guid projectId);
        Task<Temporality> GetTemporalityByIdsAsync(Guid projectId, Guid rateId);
        Task CreateTemporalityAsync(Temporality temporality);
        Task UpdateTemporalityAsync(Temporality dbTemporality, Temporality temporality);
        Task DeleteTemporalityAsync(Temporality temporality);
    }
}

