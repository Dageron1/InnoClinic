using InnoClinic.AuthService.Constants;
using InnoClinic.AuthService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InnoClinic.AuthService.Data;

public class DbInitializer : IDbInitializer
{
    private readonly AuthDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(AuthDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeAsync()
    {
        try
        {
            if ((await _db.Database.GetPendingMigrationsAsync()).Any())
            {
                await _db.Database.MigrateAsync();
            }

            if (!await _roleManager.RoleExistsAsync(Role.Patient))
            {
                await _roleManager.CreateAsync(new IdentityRole(Role.Patient));
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error initializing database: ", ex);
        }
    }
}
