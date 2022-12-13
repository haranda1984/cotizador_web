using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Services
{
    public interface IUnitService
    {
        Task<IEnumerable<Unit>> GetAllUnitsAsync();
        Task<IEnumerable<Unit>> GetAllUnitsByProjectIdAsync(Guid projectId);
        Task<Unit> GetUnitByIdAsync(Guid unitId, bool includeRelations = true);
        Task CreateUnitAsync(Unit unit);
        Task UpdateUnitAsync(Unit dbUnit, Unit unit);
        Task UpdateUnitPriceAsync(Guid unitId, decimal price);
        Task DeleteUnitAsync(Unit unit);
    }
}