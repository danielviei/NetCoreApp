using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CommentNamespace.Models
{
    public class CommentEditViewModel
    {
        [Required]
        public required string Content { get; set; }
    }
}