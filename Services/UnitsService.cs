using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitsRepository _unitRepository;

        public UnitService(IUnitsRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }

        public Task CreateUnitAsync(Unit unit)
        {
            return _unitRepository.CreateUnitAsync(unit);
        }

        public Task DeleteUnitAsync(Unit unit)
        {
            return _unitRepository.DeleteUnitAsync(unit);
        }

        public Task<IEnumerable<Unit>> GetAllUnitsAsync()
        {
            return _unitRepository.GetAllUnitsAsync();
        }

        public Task<IEnumerable<Unit>> GetAllUnitsByProjectIdAsync(Guid projectId)
        {
            return _unitRepository.GetAllUnitsByProjectIdAsync(projectId);
        }

        public Task<Unit> GetUnitByIdAsync(Guid unitId, bool includeRelations = true)
        {
            return _unitRepository.GetUnitByIdAsync(unitId, includeRelations);
        }

        public Task UpdateUnitAsync(Unit dbUnit, Unit unit)
        {
            return _unitRepository.UpdateUnitAsync(dbUnit, unit);
        }

        public Task UpdateUnitPriceAsync(Guid unitId, decimal price)
        {
            return _unitRepository.UpdateUnitPriceAsync(unitId, price);
        }
    }
}