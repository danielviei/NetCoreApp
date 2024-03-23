using PublicationNamespace.Models;
using UserNamespace.Models;

namespace AdminNamespace.Models
{
    public class PublicationWithCommentCount
    {
        public required Publication Publication { get; set; }
        public int CommentCount { get; set; }
    }
    
    public class AdminViewModel
    {
        public int UserCount { get; set; }
        public int PublicationCount { get; set; }
        public int CommentCount { get; set; }
        public CustomUser? UserWithMostPublications { get; set; }
        public CustomUser? UserWithMostComments { get; set; }
        public IEnumerable<CustomUser>? Users { get; set; }
        public IEnumerable<PublicationWithCommentCount>? Publications { get; set; }
        public int? MaxCommentCountUser { get; set; }
        public int? MaxPublicationCountUser { get; set; }
    }
}