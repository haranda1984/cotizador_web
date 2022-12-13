using System;

namespace HeiLiving.Quotes.Api.Entities
{
    public class Temporality
    {
        public Guid ProjectId { get; set; }
        public Guid RateId { get; set; }
        public int Month { get; set; }
        public decimal OccupationInDays { get; set; }
        public decimal OccupationInDaysMax { get; set; }
        public decimal Percentage { get; set; }

        //-----------------------------
        //Relationships
        public Project Project { get; set; }
        public Rate Rate { get; set; }
    }
}