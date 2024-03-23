using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CommentNamespace.Models
{
    public class CommentCreateViewModel
    {
        [Required]
        public required string Content { get; set; }

        [Required]
        public required int PublicationId { get; set; }

    }
}