using System;
using System.Collections.Generic;

namespace HeiLiving.Quotes.Api.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
    }
}