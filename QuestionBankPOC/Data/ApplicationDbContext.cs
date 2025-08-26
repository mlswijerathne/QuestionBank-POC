using Microsoft.EntityFrameworkCore;
using QuestionBankPOC.Models;

namespace QuestionBankPOC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Company configuration
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.HasIndex(e => e.Name).IsUnique();
            });
            
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirebaseUid).HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(50);
                entity.HasIndex(e => e.FirebaseUid).IsUnique();
                entity.HasIndex(e => new { e.CompanyId, e.Email }).IsUnique();
                
                entity.HasOne(e => e.Company)
                      .WithMany(c => c.Users)
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Invitation configuration  
            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(50);
                entity.Property(e => e.Token).HasMaxLength(500);
                
                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
