using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserNamespace.Models
{
    public class CustomRole : IdentityRole<int>
    {
        public const string User = "User";
        public const string Admin = "Admin";
    }

    public class CustomUser : IdentityUser<int>
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100)]
        public string? Lastname { get; set; }

        [Required]
        public string? UserRole { get; set; } = CustomRole.User;

        [NotMapped]
        public IFormFile? Image { get; set; }

        public string? ImagePath { get; set; }

        public bool IsStaff { get; set; } = false;

        public bool IsSuperuser { get; set; } = false;

        public bool IsBlocked { get; set; } = true;

        public string? GetFullName()
        {
            return $"{Name} {Lastname}";
        }

        public string? GetShortName()
        {
            return Name;
        }

        public string? GetName()
        {
            return Name;
        }

        public bool HasPerm()
        {
            return IsStaff;
        }

        public bool HasModulePerms(string appLabel)
        {
            return IsStaff;
        }
    }
}