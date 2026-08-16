using DataAccess.Dto;
using Microsoft.EntityFrameworkCore;
using PortFolioAPI.Models;

namespace PortFolioAPI.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Viewer> viewers_list { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Viewer>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).ValueGeneratedOnAdd();
            });
        }
    }
}
