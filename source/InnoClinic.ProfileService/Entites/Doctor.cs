using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace InnoClinic.ProfileService.Entites
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [ForeignKey("Account")]
        public Guid AccountId { get; set; }
        [ForeignKey("Specialization")]
        public int SpecializationId { get; set; }
        [ForeignKey("Office")]
        public int OfficeId { get; set; } 

        [Required]
        public int CareerStartYear { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        public Account Account { get; set; }
        //public Specialization Specialization { get; set; }
        //public Office Office { get; set; }
    }

}
