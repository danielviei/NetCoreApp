using System.ComponentModel.DataAnnotations;

namespace UserNamespace.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        public string? EmailError { get; set; }

        public string? PasswordError { get; set; }
    }
}