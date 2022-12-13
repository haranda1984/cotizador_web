using System;
using System.Collections.Generic;

namespace HeiLiving.Quotes.Api.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; } 
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}