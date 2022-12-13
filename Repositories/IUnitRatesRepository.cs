using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public interface IUnitRatesRepository : IBaseRepository<UnitRate>
    {
        Task<IEnumerable<UnitRate>> GetAllUnitRatesAsync();
        Task<UnitRate> GetUnitRateByIdsAsync(Guid unitId, Guid rateId);
        Task<IEnumerable<UnitRate>> GetAllUnitRatesByUnitIdAsync(Guid unitId);
        Task CreateUnitRateAsync(UnitRate unitRate);
        Task UpdateUnitRateAsync(UnitRate dbUnitRate, UnitRate unitRate);
        Task DeleteUnitRateAsync(UnitRate unitRate);
    }
}

