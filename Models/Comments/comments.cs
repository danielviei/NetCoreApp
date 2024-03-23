using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserNamespace.Models;
using PublicationNamespace.Models;

namespace CommentNamespace.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Content { get; set; }

        [Required]
        public int? AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public CustomUser? Author { get; set; }

        [Required]
        public int? PublicationId { get; set; }

        [ForeignKey("PublicationId")]
        public Publication? Publication { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}