using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeiLiving.Quotes.Api.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}