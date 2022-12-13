using System;

namespace HeiLiving.Quotes.Api.Entities
{
    public class UnitRate
    {
        public Guid UnitId { get; set; }
        public decimal? CostPerNight { get; set; }
        public decimal? BuiltUpAreaCost { get; set; }
        public decimal? TerraceAreaCost { get; set; }

        //-----------------------------
        //Relationships
        public Unit Unit { get; set; }
    }
}