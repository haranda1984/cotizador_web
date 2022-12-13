using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class UnitRatesService : IUnitRatesService
    {
        private readonly IUnitRatesRepository _unitRatesRepository;

        public UnitRatesService(IUnitRatesRepository stageRepository)
        {
            _unitRatesRepository = stageRepository;
        }
        public Task CreateUnitRateAsync(UnitRate stage)
        {
            return _unitRatesRepository.CreateUnitRateAsync(stage);
        }

        public Task UpdateUnitRateAsync(UnitRate dbstage, UnitRate stage)
        {
            return _unitRatesRepository.UpdateUnitRateAsync(dbstage, stage);
        }
        public Task DeleteUnitRateAsync(UnitRate stage)
        {
            return _unitRatesRepository.DeleteUnitRateAsync(stage);
        }

        public Task<IEnumerable<UnitRate>> GetAllUnitRatesAsync()
        {
            return _unitRatesRepository.GetAllUnitRatesAsync();
        }

        public Task<UnitRate> GetUnitRateByIdsAsync(Guid unitId, Guid rateId)
        {
            return _unitRatesRepository.GetUnitRateByIdsAsync(unitId, rateId);
        }

        public Task<IEnumerable<UnitRate>> GetAllUnitRatesByUnitIdAsync(Guid unitId)
        {
            return _unitRatesRepository.GetAllUnitRatesByUnitIdAsync(unitId);
        }
    }
}