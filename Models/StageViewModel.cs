using System;

namespace HeiLiving.Quotes.Api.Models
{
    public class StageViewModel
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}