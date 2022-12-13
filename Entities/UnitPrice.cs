using System;

namespace HeiLiving.Quotes.Api.Entities
{
    public class UnitPrice
    {
        public Guid UnitId { get; set; }
        public Guid StageId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }

        //-----------------------------
        //Relationships
        public Unit Unit { get; set; }
        public Stage Stage { get; set; }
    }
}