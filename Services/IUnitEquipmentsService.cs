using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Services
{
    public interface IUnitEquipmentsService
    {
        Task<IEnumerable<UnitEquipment>> GetAllUnitEquipmentsAsync();
        Task<UnitEquipment> GetUnitEquipmentByIdAsync(Guid unitEquipmentsId);
        Task<IEnumerable<UnitEquipment>> GetUnitEquipmentsByDescriptionAsync(string description);
        Task<IEnumerable<UnitEquipment>> GetAllUnitEquipmentsByProjectIdAsync(Guid projectId);
        Task CreateUnitEquipmentAsync(UnitEquipment unitEquipment);
        Task UpdateUnitEquipmentAsync(UnitEquipment dbUnitEquipment, UnitEquipment unitEquipment);
        Task DeleteUnitEquipmentAsync(UnitEquipment unitEquipment);
    }
}

