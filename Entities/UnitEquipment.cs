using System;

namespace HeiLiving.Quotes.Api.Entities
{
    public class UnitEquipment
    {
        public Guid Id { get; set; }
        public Guid UnitId { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }

        //-----------------------------
        //Relationships
        public Unit Unit { get; set; }
    }
}