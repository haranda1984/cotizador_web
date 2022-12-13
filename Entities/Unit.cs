using System;
using System.Collections.Generic;

namespace HeiLiving.Quotes.Api.Entities
{
    public class Unit
    {
        public Guid Id { get; set; }
        public Guid ModelId { get; set; }        
        public string Number { get; set; }
        public string Building { get; set; }
        public int? Level { get; set; }
        public decimal GrossArea { get; set; }
        public decimal BuiltUpArea { get; set; }
        public decimal TerraceArea { get; set; }
        public UnitStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public ICollection<UnitPrice> UnitPrices { get; set; }
        public bool IsCondoHotel { get; set; }

        //-----------------------------
        //Relationships

        public Model Model { get; set; }
    }

    public enum UnitStatus {
        ForSale = 0,
        Blocked = 1,
        Sold = 2
    }
}