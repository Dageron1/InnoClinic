using InnoClinic.AuthService.Constants;
using InnoClinic.AuthService.DTOs;
using InnoClinic.AuthService.Entities;
using InnoClinic.AuthService.Services.Error;
using InnoClinic.AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InnoClinic.AuthService.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<string> RegisterAsync(RegistrationRequestDto registrationRequestDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registrationRequestDto.Email.ToLower());

        if (existingUser is not null)
        {
            throw new AuthException(ErrorCode.UserAlreadyExists);
        }

        ApplicationUser applicationUser = new()
        {
            UserName = registrationRequestDto.Email,
            Email = registrationRequestDto.Email,
            NormalizedEmail = registrationRequestDto.Email.ToUpper(),
            PhoneNumber = registrationRequestDto.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
        };

        await _userManager.CreateAsync(applicationUser, registrationRequestDto.Password);

        var roleName = Role.Patient;

        await _userManager.AddToRoleAsync(applicationUser, roleName);
        var roles = await _userManager.GetRolesAsync(applicationUser);

        //await SendEmailConfirmationAsync(applicationUser.Email);

        return _tokenService.GenerateToken(applicationUser, roles);
    }

    public async Task<string> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(loginRequestDto.Email);

        if (existingUser is null)
        {
            throw new AuthException(ErrorCode.InvalidCredentials);
        }

        bool isValidPassword = await _userManager.CheckPasswordAsync(existingUser, loginRequestDto.Password);

        if (!isValidPassword)
        {
            throw new AuthException(ErrorCode.InvalidCredentials);
        }

        var roles = await _userManager.GetRolesAsync(existingUser);

        return _tokenService.GenerateToken(existingUser, roles);
    }
}
