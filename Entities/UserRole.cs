using System;

namespace HeiLiving.Quotes.Api.Entities
{
    public class UserRole
    {        
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        
        //-----------------------------
        //Relationships
        public User User { get; set; }
        public Role Role { get; set; }
    }
}