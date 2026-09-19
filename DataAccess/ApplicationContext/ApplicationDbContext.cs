using DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace PortFolioAPI.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Viewer> viewers_list { get; set; }
        public DbSet<BrevoMailLogs> brevo_mail_logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Viewer>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<BrevoMailLogs>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).ValueGeneratedOnAdd();
            });
        }
    }
}
