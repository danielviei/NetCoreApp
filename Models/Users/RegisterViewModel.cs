using System.ComponentModel.DataAnnotations;

namespace UserNamespace.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string? PasswordConfirm { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Lastname { get; set; }

        public IFormFile? Img { get; set; }
    }
}