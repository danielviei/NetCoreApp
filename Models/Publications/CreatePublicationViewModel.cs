using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PublicationNamespace.Models
{
    public class CreatePublicationViewModel
    {
        [Required(ErrorMessage = "El título es requerido.")]
        public required string Title { get; set; }
        [Required(ErrorMessage = "La imagen es requerida.")]
        public required IFormFile? Image { get; set; }
        [Required(ErrorMessage = "El Contenido es requerido.")]
        public required string Content { get; set; }
        public int? AuthorId { get; set; }
    }
}