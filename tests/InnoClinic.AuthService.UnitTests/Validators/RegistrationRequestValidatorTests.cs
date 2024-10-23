using FluentValidation.TestHelper;
using InnoClinic.AuthService.DTOs;
using InnoClinic.AuthService.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoClinic.AuthService.UnitTests.Validators
{
    public class RegistrationRequestValidatorTests
    {
        private readonly RegistrationRequestValidator _validator;

        public RegistrationRequestValidatorTests()
        {
            _validator = new RegistrationRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            // Arrange
            var model = new RegistrationRequestDto { Email = "", Password = "ValidPassword123!", PhoneNumber = "1234567890" };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Email is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            // Arrange
            var model = new RegistrationRequestDto { Email = "invalid-email", Password = "ValidPassword123!", PhoneNumber = "1234567890" };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("Invalid email format.");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            // Arrange
            var model = new RegistrationRequestDto { Email = "testuser@example.com", Password = "", PhoneNumber = "1234567890" };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            // Arrange
            var model = new RegistrationRequestDto { Email = "testuser@example.com", Password = "short", PhoneNumber = "1234567890" };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password must be at least 8 characters long.");
        }

        [Fact]
        public void Should_Have_Error_When_PhoneNumber_Is_Invalid()
        {
            // Arrange
            var model = new RegistrationRequestDto { Email = "testuser@example.com", Password = "ValidPassword123!", PhoneNumber = "12345" };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
                .WithErrorMessage("Phone number must be 10 digits long.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            // Arrange
            var model = new RegistrationRequestDto
            {
                Email = "testuser@example.com",
                Password = "ValidPassword123!",
                PhoneNumber = "1234567890"
            };

            // Act & Assert
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
