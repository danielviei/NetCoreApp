using System.ComponentModel.DataAnnotations;

namespace AdminNamespace.Models
{
    public class EditUserModel
    {
        public IFormFile? Image { get; set; }
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Lastname { get; set; }

        public string? Email { get; set; }

        public string? ImagePath { get; set; }
        public required bool IsStaff { get; set; }

        public bool IsBlocked { get; set; }
    }
}