using System;

namespace HeiLiving.Quotes.Api.Models
{
    public class UnitEquipmentViewModel
    {
        public Guid Id { get; set; }
        public Guid UnitId { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
    }
}