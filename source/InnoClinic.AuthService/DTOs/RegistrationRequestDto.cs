namespace InnoClinic.AuthService.DTOs;

public class RegistrationRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
}
