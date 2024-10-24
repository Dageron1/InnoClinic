using FluentAssertions;
using InnoClinic.AuthService.Constants;
using InnoClinic.AuthService.DTOs;
using InnoClinic.AuthService.Entities;
using InnoClinic.AuthService.IntegrationTests.Factories;
using InnoClinic.AuthService.Services.Error;
using InnoClinic.AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace InnoClinic.AuthService.IntegrationTests.Services;

public class AuthServiceTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private IServiceScope _scope = null!;
    private InnoClinic.AuthService.Services.AuthService _authService = null!;
    private readonly CustomWebApplicationFactory _factory;
    private UserManager<ApplicationUser> _userManager = null!;
    private ITokenService _tokenService = null!;
    private List<string> _createdUserEmails = new();

    public AuthServiceTests(CustomWebApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public async Task InitializeAsync()
    {
        _scope = _factory.Services.CreateScope();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _tokenService = _scope.ServiceProvider.GetRequiredService<ITokenService>();
        _authService = new InnoClinic.AuthService.Services.AuthService(_userManager, _tokenService);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task RegisterAsync_CreatesUserAndReturnsToken_WhenValidDataProvided()
    {
        // Arrange
        var registrationRequest = new RegistrationRequestDto
        {
            Email = $"testuser{Guid.NewGuid()}@example.com",
            Password = "ValidPassword123!",
            PhoneNumber = "1234567890"
        };
        _createdUserEmails.Add(registrationRequest.Email);

        // Act
        var token = await _authService.RegisterAsync(registrationRequest, CancellationToken.None);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var user = await _userManager.FindByEmailAsync(registrationRequest.Email);

        user!.Email.Should().Be(registrationRequest.Email);

        var roles = await _userManager.GetRolesAsync(user);
        roles.Should().Contain(Roles.Patient);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsUserAlreadyExists_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = await CreateTestUserAsync();

        var registrationRequest = new RegistrationRequestDto
        {
            Email = existingUser.Email!,
            Password = "ValidPassword123!",
            PhoneNumber = "1234567890"
        };

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(registrationRequest, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthException>().Where(e => e.ErrorCode == ErrorCode.UserAlreadyExists);
    }

    [Fact]
    public async Task RegisterAsync_GeneratesTokenWithRoles()
    {
        // Arrange
        var registrationRequest = new RegistrationRequestDto
        {
            Email = $"testuser{Guid.NewGuid()}@example.com",
            Password = "ValidPassword123!",
            PhoneNumber = "1234567890"
        };
        _createdUserEmails.Add(registrationRequest.Email);

        // Act
        var token = await _authService.RegisterAsync(registrationRequest, CancellationToken.None);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var user = await _userManager.FindByEmailAsync(registrationRequest.Email);
        user.Should().NotBeNull();

        var roles = await _userManager.GetRolesAsync(user);
        roles.Should().Contain(Roles.Patient);

        var claimsPrincipal = _tokenService!.ValidateToken(token);

        var claims = claimsPrincipal.Claims;
        claims.Any(c => c.Value.Contains(registrationRequest.Email)).Should().BeTrue();
        claims.Any(c => c.Value.Contains(Roles.Patient)).Should().BeTrue();
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var existingUser = await CreateTestUserAsync();

        var loginRequest = new LoginRequestDto
        {
            Email = existingUser.Email!,
            Password = "ValidPassword123!"
        };

        // Act
        var token = await _authService.LoginAsync(loginRequest, CancellationToken.None);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var claimsPrincipal = _tokenService.ValidateToken(token);

        var claims = claimsPrincipal.Claims;
        claims.Any(c => c.Value.Contains(loginRequest.Email)).Should().BeTrue();
    }

    [Fact]
    public async Task LoginAsync_ThrowsInvalidCredentials_WhenEmailDoesNotExist()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "SomePassword123!"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginRequest, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthException>().Where(e => e.ErrorCode == ErrorCode.InvalidCredentials);
    }

    [Fact]
    public async Task LoginAsync_ThrowsInvalidCredentials_WhenPasswordIsInvalid()
    {
        // Arrange
        var existingUser = await CreateTestUserAsync();

        var loginRequest = new LoginRequestDto
        {
            Email = existingUser.Email!,
            Password = "InvalidPassword123!"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginRequest, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthException>().Where(e => e.ErrorCode == ErrorCode.InvalidCredentials);
    }

    public async Task DisposeAsync()
    {
        foreach (var email in _createdUserEmails)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                await _userManager.DeleteAsync(user);
            }
        }
        _scope.Dispose();
    }

    private async Task<ApplicationUser> CreateTestUserAsync()
    {
        var user = new ApplicationUser
        {
            Email = $"testuser{Guid.NewGuid()}@example.com",
            UserName = $"{Guid.NewGuid():N}",
            PhoneNumber = "1234567890"
        };
        var createUserResult = await _userManager.CreateAsync(user, "ValidPassword123!");

        if (!createUserResult.Succeeded)
        {
            throw new Exception("Failed to create test user.");
        }

        _createdUserEmails.Add(user.Email);
        return user;
    }
}
