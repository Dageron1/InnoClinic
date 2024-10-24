using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;
using InnoClinic.AuthService.Constants;
using InnoClinic.AuthService.DTOs;
using InnoClinic.AuthService.Entities;
using InnoClinic.AuthService.Services.Error;
using InnoClinic.AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace InnoClinic.AuthService.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly InnoClinic.AuthService.Services.AuthService _authService;
    private readonly IFixture _fixture;

    public AuthServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _userManagerMock = _fixture.Freeze<Mock<UserManager<ApplicationUser>>>();
        _tokenServiceMock = _fixture.Freeze<Mock<ITokenService>>();
        _authService = new InnoClinic.AuthService.Services.AuthService(_userManagerMock.Object, _tokenServiceMock.Object);
    }

    [Theory, AutoData]
    public async Task RegisterAsync_ShouldThrowAuthException_WhenUserAlreadyExists(RegistrationRequestDto registrationRequestDto, ApplicationUser existingUser)
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(registrationRequestDto, It.IsAny<CancellationToken>());

        // Assert
        var exception = await act.Should().ThrowAsync<AuthException>();
        exception.Which.ErrorCode.Should().Be(ErrorCode.UserAlreadyExists);

        _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task RegisterAsync_ShouldCreateNewUserAndAddToRole(RegistrationRequestDto registrationRequestDto)
    {
        // Arrange
        ApplicationUser createdUser = null!;
        var generatedToken = "generated-jwt-token";
        var roleName = Roles.Patient;

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .Callback<ApplicationUser, string>((user, password) => createdUser = user)
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { roleName });

        _tokenServiceMock
            .Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns(generatedToken);

        // Act
        var result = await _authService.RegisterAsync(registrationRequestDto, It.IsAny<CancellationToken>());

        // Assert
        createdUser.Email.Should().Be(registrationRequestDto.Email);
        createdUser.PhoneNumber.Should().Be(registrationRequestDto.PhoneNumber);
        result.Should().Be(generatedToken);

        _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), roleName), Times.Once);
        _userManagerMock.Verify(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Once);
        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task LoginAsync_ShouldThrowAuthException_WhenUserDoesNotExist(LoginRequestDto loginRequestDto)
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginRequestDto, It.IsAny<CancellationToken>());

        // Assert
        var exception = await act.Should().ThrowAsync<AuthException>();
        exception.Which.ErrorCode.Should().Be(ErrorCode.InvalidCredentials);

        _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task LoginAsync_ShouldThrowAuthException_WhenPasswordIsInvalid(LoginRequestDto loginRequestDto, ApplicationUser existingUser)
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(existingUser, It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginRequestDto, It.IsAny<CancellationToken>());

        // Assert
        var exception = await act.Should().ThrowAsync<AuthException>();
        exception.Which.ErrorCode.Should().Be(ErrorCode.InvalidCredentials);

        _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(x => x.CheckPasswordAsync(existingUser, It.IsAny<string>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid(LoginRequestDto loginRequestDto, ApplicationUser existingUser, string token)
    {
        // Arrange
        var roles = _fixture.Create<List<string>>();

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(existingUser, It.IsAny<string>()))
            .ReturnsAsync(true);

        _userManagerMock
            .Setup(x => x.GetRolesAsync(existingUser))
            .ReturnsAsync(roles);

        _tokenServiceMock
            .Setup(x => x.GenerateToken(existingUser, roles))
            .Returns(token);

        // Act
        var result = await _authService.LoginAsync(loginRequestDto, It.IsAny<CancellationToken>());

        // Assert
        result.Should().Be(token);

        _userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(x => x.CheckPasswordAsync(existingUser, It.IsAny<string>()), Times.Once);
        _userManagerMock.Verify(x => x.GetRolesAsync(existingUser), Times.Once);
        _tokenServiceMock.Verify(x => x.GenerateToken(existingUser, roles), Times.Once);
    }
}
