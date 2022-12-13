using System;
using System.ComponentModel.DataAnnotations;

namespace HeiLiving.Quotes.Api.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}