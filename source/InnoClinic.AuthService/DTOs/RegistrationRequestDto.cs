namespace InnoClinic.AuthService.DTOs;

public class RegistrationRequestDto
{
    public string Email { get; init; }
    public string Password { get; init; }
    public string PhoneNumber { get; init; }
}
