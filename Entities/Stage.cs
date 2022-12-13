using System;
using System.Collections.Generic;

namespace HeiLiving.Quotes.Api.Entities
{
    public class Stage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public short Order { get; set; }
        public ICollection<ProjectStage> ProjectStages { get; set; }
        public ICollection<UnitPrice> UnitPrices { get; set; }
    }
}