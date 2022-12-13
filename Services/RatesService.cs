using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class RatesService : IRatesService
    {
        private readonly IRatesRepository _ratesRepository;

        public RatesService(IRatesRepository stageRepository)
        {
            _ratesRepository = stageRepository;
        }
        
        public Task<Rate> GetRateByIdAsync(Guid stageId)
        {
            return _ratesRepository.GetRateByIdAsync(stageId);
        }

        public Task<Rate> GetRateByNameAsync(string name)
        {
            return _ratesRepository.GetRateByNameAsync(name);
        }

        public Task CreateRateAsync(Rate stage)
        {
            return _ratesRepository.CreateRateAsync(stage);
        }

        public Task UpdateRateAsync(Rate dbstage, Rate stage)
        {
            return _ratesRepository.UpdateRateAsync(dbstage, stage);
        }
        public Task DeleteRateAsync(Rate stage)
        {
            return _ratesRepository.DeleteRateAsync(stage);
        }

        public Task<IEnumerable<Rate>> GetAllRatesAsync()
        {
            return _ratesRepository.GetAllRatesAsync();
        }
    }
}