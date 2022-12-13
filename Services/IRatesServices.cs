using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Services
{
    public interface IRatesService
    {
        Task<IEnumerable<Rate>> GetAllRatesAsync();
        Task<Rate> GetRateByIdAsync(Guid ratesId);
        Task<Rate> GetRateByNameAsync(string name);
        Task CreateRateAsync(Rate rate);
        Task UpdateRateAsync(Rate dbRate, Rate rate);
        Task DeleteRateAsync(Rate rate);
    }
}

