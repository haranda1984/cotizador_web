using System;

namespace HeiLiving.Quotes.Api.Entities
{
    public class Model
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
    }
}