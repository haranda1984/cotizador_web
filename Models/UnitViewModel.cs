using System;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Models
{
    public class UnitViewModel
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public decimal Price { get; set; }
        public decimal MinimumExpectedValue { get; set; }
        public string Number { get; set; }
        public int? Level { get; set; }
        public decimal GrossArea { get; set; }
        public decimal BuiltUpArea { get; set; }
        public decimal TerraceArea { get; set; }
        public UnitStatus Status { get; set; }
        public bool IsActive { get; set; }
        public bool AllowsCondoHotel { get; set; }
        public bool IsCondoHotel { get; set; }
        public bool IsCondoHotelOptional { get; set; }
        public decimal EquipmentPrice { get; set; }
        public decimal PreOperatingCost { get; set; }
        public decimal ProjectAppreciation { get; set; }
        public string Currency { get; set; }
    }
}