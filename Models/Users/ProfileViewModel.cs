namespace UserNamespace.Models
{
    public class ProfileViewModel
    {
        public IFormFile? ImageFile { get; set; }
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Lastname { get; set; }

        public string? Email { get; set; }

        public string? ImagePath { get; set; }
    }
}