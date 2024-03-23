using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using UserNamespace.Models;
using CommentNamespace.Models;

namespace PublicationNamespace.Models
{
    public class Publication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? Title { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public string? Content { get; set; }

        [Required]
        public int? AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public CustomUser? Author { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public bool IsBlocked { get; set; } = false;

        [NotMapped]
        public ICollection<Comment>? Comments { get; set; }
    }
}