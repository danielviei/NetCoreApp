using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PublicationNamespace.Models;
using CommentNamespace.Models;
using UserNamespace.Models;

namespace ApplicationDbContextNamespace.Data
{
    public class ApplicationDbContext : IdentityDbContext<CustomUser, CustomRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Publication> Publications { get; set; }
        public DbSet<Comment> Comments { get; set; }
        // No necesitas este DbSet porque IdentityDbContext ya lo incluye
        // public DbSet<CustomUser> Users { get; set; }
    }
}