using System;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Models
{
    public class CreateUnitRequest
    {
        public Guid Id { get; set; }
        public Guid ModelId { get; set; }
        public string Number { get; set; }
        public int? Level { get; set; }
        public decimal GrossArea { get; set; }
        public decimal BuiltUpArea { get; set; }
        public decimal TerraceArea { get; set; }
        public UnitStatus Status { get; set; }
    }
}