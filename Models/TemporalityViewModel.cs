using System;

namespace HeiLiving.Quotes.Api.Models
{
    public class TemporalityViewModel
    {
        public Guid ProjectId { get; set; }
        public Guid RateId { get; set; }
        public int Month { get; set; }
        public decimal OccupationInDays { get; set; }
    }
}