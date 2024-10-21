using InnoClinic.AuthService.Entities;

namespace InnoClinic.AuthService.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
}
