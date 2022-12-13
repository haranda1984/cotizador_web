using System.ComponentModel.DataAnnotations;

namespace HeiLiving.Quotes.Api.Models
{
    public class UserAuthViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Must be between 8 and 256 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}