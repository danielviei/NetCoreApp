using System.ComponentModel.DataAnnotations;

namespace UserNamespace.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
