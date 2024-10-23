using InnoClinic.AuthService.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InnoClinic.AuthService.Data;

public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
       : base(options)
    {
    }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
           .HasIndex(u => u.Email)
           .IsUnique();

        modelBuilder.Entity<ApplicationUser>()
           .Property(u => u.CreatedBy)
           .IsRequired(false);

        modelBuilder.Entity<ApplicationUser>()
               .Property(u => u.UpdatedBy)
               .IsRequired(false);

        modelBuilder.Entity<ApplicationUser>()
               .Property(u => u.PhotoId)
               .IsRequired(false);
    }
}
