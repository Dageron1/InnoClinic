using InnoClinic.AuthService.DTOs;
using InnoClinic.AuthService.Entities;

namespace InnoClinic.AuthService.Services.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegistrationRequestDto registrationRequestDto);
    Task<string> LoginAsync(LoginRequestDto loginRequestDto);
}
