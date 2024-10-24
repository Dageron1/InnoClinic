using InnoClinic.AuthService.DTOs;

namespace InnoClinic.AuthService.Services.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegistrationRequestDto registrationRequestDto, CancellationToken cancellationToken);
    Task<string> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken);
}

