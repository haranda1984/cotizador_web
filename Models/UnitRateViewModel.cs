using System;

namespace HeiLiving.Quotes.Api.Models
{
    public class UnitRateViewModel
    {
        public Guid UnitId { get; set; }
        public decimal? CostPerNight { get; set; }
        public decimal? BuiltUpAreaCost { get; set; }
        public decimal? TerraceAreaCost { get; set; }
    }
}