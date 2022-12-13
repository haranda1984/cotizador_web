using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;
using HeiLiving.Quotes.Api.Repositories;

namespace HeiLiving.Quotes.Api.Services
{
    public class UnitEquipmentsService : IUnitEquipmentsService
    {
        private readonly IUnitEquipmentsRepository _unitEquipmentsRepository;

        public UnitEquipmentsService(IUnitEquipmentsRepository stageRepository)
        {
            _unitEquipmentsRepository = stageRepository;
        }

        public Task<IEnumerable<UnitEquipment>> GetAllUnitEquipmentsAsync()
        {
            return _unitEquipmentsRepository.GetAllUnitEquipmentsAsync();
        }

        public Task<UnitEquipment> GetUnitEquipmentByIdAsync(Guid unitEquipmentId)
        {
            return _unitEquipmentsRepository.GetUnitEquipmentByIdAsync(unitEquipmentId);
        }

        public Task<IEnumerable<UnitEquipment>> GetUnitEquipmentsByDescriptionAsync(string description)
        {
            return _unitEquipmentsRepository.GetUnitEquipmentsByDescriptionAsync(description);
        }

        public Task<IEnumerable<UnitEquipment>> GetAllUnitEquipmentsByProjectIdAsync(Guid projectId)
        {
            return _unitEquipmentsRepository.GetAllUnitEquipmentsByProjectIdAsync(projectId);
        }

        public Task CreateUnitEquipmentAsync(UnitEquipment unitEquipment)
        {
            return _unitEquipmentsRepository.CreateUnitEquipmentAsync(unitEquipment);
        }

        public Task UpdateUnitEquipmentAsync(UnitEquipment dbUnitEquipment, UnitEquipment unitEquipment)
        {
            return _unitEquipmentsRepository.UpdateUnitEquipmentAsync(dbUnitEquipment, unitEquipment);
        }

        public Task DeleteUnitEquipmentAsync(UnitEquipment unitEquipment)
        {
            return _unitEquipmentsRepository.DeleteUnitEquipmentAsync(unitEquipment);
        }
    }
}