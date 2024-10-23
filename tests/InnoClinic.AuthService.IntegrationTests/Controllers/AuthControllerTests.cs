using InnoClinic.AuthService.DTOs;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InnoClinic.AuthService.IntegrationTests.Factories;
using InnoClinic.AuthService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace InnoClinic.AuthService.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private IServiceScope _scope = null!;
    private UserManager<ApplicationUser> _userManager = null!;
    private readonly List<string> _createdUserEmails = new();

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _scope = _factory.Services.CreateScope();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task Register_Returns201Created_WhenUserIsRegistered()
    {
        // Arrange
        var registrationRequest = new RegistrationRequestDto
        {
            Email = $"testuser{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!",
            PhoneNumber = "1234567890" 
        };
        _createdUserEmails.Add(registrationRequest.Email);

        var content = new StringContent(JsonConvert.SerializeObject(registrationRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("token");

        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == registrationRequest.Email);
        user!.Email.Should().Be(registrationRequest.Email);
    }

    [Fact]
    public async Task Register_Returns409Conflict_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = await CreateTestUserAsync();

        var registrationRequest = new RegistrationRequestDto
        {
            Email = existingUser.Email!,
            Password = "TestPassword123!",
            PhoneNumber = "1234567890"
        };

        var content = new StringContent(JsonConvert.SerializeObject(registrationRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var responseBody = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseBody);

        problemDetails!.Status.Should().Be(StatusCodes.Status409Conflict);
        problemDetails.Title.Should().Be("UserAlreadyExists");
        problemDetails.Detail.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_Returns400BadRequest_WhenEmailIsEmpty()
    {
        // Arrange
        var registrationRequest = new RegistrationRequestDto
        {
            Email = "",
            Password = "ValidPassword123!",
            PhoneNumber = "1234567890"
        };

        var content = new StringContent(JsonConvert.SerializeObject(registrationRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Email is required.");
    }

    [Fact]
    public async Task Register_Returns400BadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        var registrationRequest = new RegistrationRequestDto
        {
            Email = "invalid-email",
            Password = "ValidPassword123!",
            PhoneNumber = "1234567890"
        };

        var content = new StringContent(JsonConvert.SerializeObject(registrationRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Invalid email format.");
    }

    [Fact]
    public async Task Register_Returns400BadRequest_WhenPasswordIsEmpty()
    {
        // Arrange
        var registrationRequest = new RegistrationRequestDto
        {
            Email = "testuser@example.com",
            Password = "",
            PhoneNumber = "1234567890"
        };

        var content = new StringContent(JsonConvert.SerializeObject(registrationRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Password is required.");
    }

    [Fact]
    public async Task Register_Returns400BadRequest_WhenPasswordIsTooShort()
    {
        // Arrange
        var registrationRequest = new RegistrationRequestDto
        {
            Email = "testuser@example.com",
            Password = "short",
            PhoneNumber = "1234567890"
        };

        var content = new StringContent(JsonConvert.SerializeObject(registrationRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Password must be at least 8 characters long.");
    }

    [Fact]
    public async Task Register_Returns400BadRequest_WhenPhoneNumberIsEmpty()
    {
        // Arrange
        var registrationRequest = new RegistrationRequestDto
        {
            Email = "testuser@example.com",
            Password = "ValidPassword123!",
            PhoneNumber = ""
        };

        var content = new StringContent(JsonConvert.SerializeObject(registrationRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Phone number is required.");
    }


    [Fact]
    public async Task Login_Returns200OK_WhenCredentialsAreValid()
    {
        // Arrange
        var existingUser = await CreateTestUserAsync();

        var loginRequest = new LoginRequestDto
        {
            Email = existingUser.Email!,
            Password = "ValidPassword123!"
        };

        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/sessions", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("token");
    }

    [Fact]
    public async Task Login_Returns401Unauthorized_WhenPasswordIsInvalid()
    {
        // Arrange
        var existingUser = await CreateTestUserAsync();

        var loginRequest = new LoginRequestDto
        {
            Email = existingUser.Email!,
            Password = "InvalidPassword123!" // Invalid password
        };

        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/sessions", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var responseBody = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseBody);

        problemDetails!.Status.Should().Be(StatusCodes.Status401Unauthorized);
        problemDetails.Title.Should().Be("InvalidCredentials");
    }

    [Fact]
    public async Task Login_Returns401Unauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "SomePassword123!"
        };

        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/sessions", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var responseBody = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseBody);

        problemDetails!.Status.Should().Be(StatusCodes.Status401Unauthorized);
        problemDetails.Title.Should().Be("InvalidCredentials");
    }

    [Fact]
    public async Task Login_Returns400BadRequest_WhenEmailIsEmpty()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "",
            Password = "ValidPassword123!"
        };

        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/sessions", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Email is required.");
    }

    [Fact]
    public async Task Login_Returns400BadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "invalid-email-format",
            Password = "ValidPassword123!"
        };

        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/sessions", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Invalid email format.");
    }

    [Fact]
    public async Task Login_Returns400BadRequest_WhenPasswordIsEmpty()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Email = "testuser@example.com",
            Password = ""
        };

        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/sessions", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Password is required.");
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
            PhoneNumber = "+1234567890"
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
