using InnoClinic.AuthService.DTOs;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using System.Text;
using InnoClinic.AuthService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InnoClinic.AuthService.IntegrationTests.Factories;

namespace InnoClinic.AuthService.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private IServiceScope? _scope;
    private AuthDbContext? _dbContext;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _scope = _factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await CustomWebApplicationFactory.InitializeTestDatabaseAsync(_scope.ServiceProvider);
    }

    [Fact]
    public async Task Register_Returns201Created_WhenUserIsRegistered()
    {
        // Arrange

        var registrationRequest = new RegistrationRequestDto
        {
            Email = "testuser@example.com",
            Password = "TestPassword123!",
            PhoneNumber = "1234567890" 
        };

        var content = new StringContent(JsonConvert.SerializeObject(registrationRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("token");

        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == "testuser@example.com");
        user.Should().NotBeNull();
        user!.Email.Should().Be("testuser@example.com");
    }

    public async Task DisposeAsync()
    {
        if (_dbContext is not null)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == "testuser@example.com");

            if (user is not null)
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }
        _scope?.Dispose();
    }
}
