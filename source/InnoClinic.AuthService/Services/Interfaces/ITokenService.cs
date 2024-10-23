using InnoClinic.AuthService.Entities;
using System.Security.Claims;

namespace InnoClinic.AuthService.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    ClaimsPrincipal ValidateToken(string token);
}
