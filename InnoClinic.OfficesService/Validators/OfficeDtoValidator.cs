using FluentValidation;
using InnoClinic.OfficesService.DTOs;

namespace InnoClinic.OfficesService.Validators;

public class OfficeDtoValidator : AbstractValidator<OfficeDto>
{
    public OfficeDtoValidator()
    {
        RuleFor(office => office.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MinimumLength(7).WithMessage("Address must be at least 7 characters long.");

        RuleFor(office => office.PhotoId)
            .NotEmpty().WithMessage("PhotoId is required.");

        RuleFor(office => office.RegistryPhoneNumber)
            .NotEmpty().WithMessage("Registry phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

        RuleFor(office => office.IsActive)
            .NotNull().WithMessage("IsActive status must be specified.");
    }
}
