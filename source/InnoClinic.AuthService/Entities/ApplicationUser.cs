using Microsoft.AspNetCore.Identity;

namespace InnoClinic.AuthService.Entities;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public int? PhotoId { get; set; }
}
