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
    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
           .HasIndex(u => u.Email)
           .IsUnique();

        builder.Entity<ApplicationUser>()
           .Property(u => u.CreatedBy)
           .IsRequired(false);

        builder.Entity<ApplicationUser>()
               .Property(u => u.UpdatedBy)
               .IsRequired(false);

        builder.Entity<ApplicationUser>()
               .Property(u => u.PhotoId)
               .IsRequired(false);
    }
}
