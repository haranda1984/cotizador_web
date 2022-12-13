using System;
using System.ComponentModel.DataAnnotations;

namespace HeiLiving.Quotes.Api.Models
{
    public class RoleViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; } 
    }
}