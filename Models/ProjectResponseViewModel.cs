using System;
using System.Collections.Generic;

namespace HeiLiving.Quotes.Api.Models
{
    public class ProjectResponseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public string ThemeColor { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal Appreciation { get; set; }
        public bool AllowsCondoHotel { get; set; }
        public int MinimumCondoHotelUnits { get; set; }
        public decimal? CondoHotelPreOperatingCost { get; set; }
        public string CurrentStage { get; set; }
        public Guid CurrentStageId { get; set; }
        public int UnitsAvailables { get; set; }
        public int UnitsSoldAsCondoHotel { get; set; }
        public bool IsCondoHotelOptional { get; set; }
        public int UnitsNumber { get; set; }
        public int TradePoliciesNumber { get; set; }
        public ICollection<StageViewModel> Stages { get; set; }
        public decimal? CapRate { get; set; }
    }
}