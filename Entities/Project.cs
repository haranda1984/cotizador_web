using System;
using System.Collections.Generic;

namespace HeiLiving.Quotes.Api.Entities
{
    public class Project
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
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public ICollection<(string Name, string ImageUrl)> Amenities { get; set; }
        public bool AllowsCondoHotel { get; set; }
        public int MinimumCondoHotelUnits { get; set; }
        public decimal Appreciation { get; set; }
        public decimal? CondoHotelPreOperatingCost { get; set; }
        public Guid CurrentStageId { get; set; }
        public decimal? CapRate { get; set; }

        //-----------------------------
        //Relationships        
        public Stage CurrentStage { get; set; }
        public ICollection<ProjectStage> ProjectStages { get; set; }
    }
}