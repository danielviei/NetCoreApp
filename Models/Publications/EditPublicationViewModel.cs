using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PublicationNamespace.Models
{
    public class EditPublicationViewModel
    {
        [Required(ErrorMessage = "El título es requerido.")]
        public string? Title { get; set; }

        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "El Contenido es requerido.")]
        public string? Content { get; set; }

        public int? AuthorId { get; set; }

        public int Id { get; set; }

        public string? ImagePath { get; set; }

        public bool IsBlocked { get; set; }
    }
}