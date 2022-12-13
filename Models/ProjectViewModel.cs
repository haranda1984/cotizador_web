using System;
using System.Collections.Generic;

namespace HeiLiving.Quotes.Api.Models
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public string ThemeColor { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public bool AllowsCondoHotel { get; set; }
        public int MinimumCondoHotelUnits { get; set; }
        public decimal Appreciation { get; set; }
        public Guid CurrentStageId { get; set; }
        public decimal? CapRate { get; set; }
    }
}