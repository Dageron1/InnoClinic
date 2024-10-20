using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InnoClinic.ProfileService.Entites;

public class Receptionist
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }
    [MaxLength(50)]
    public string MiddleName { get; set; }
    [ForeignKey("Account")]
    public Guid AccountId { get; set; } 

    [ForeignKey("Office")]
    public int OfficeId { get; set; } 

    public Account Account { get; set; }
    //public Office Office { get; set; }
}
