using System;
using System.Collections.Generic;

namespace HeiLiving.Quotes.Api.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public UserProfile Profile { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public int FailedLoginAttempts { get; set; }
        public int SuccessfulLoginAttempts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}