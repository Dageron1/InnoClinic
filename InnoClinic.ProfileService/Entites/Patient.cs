using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace InnoClinic.ProfileService.Entites
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public bool IsLinkedToAccount { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [ForeignKey("Account")]
        public Guid AccountId { get; set; }

        // Навигационные свойства
        public Account Account { get; set; }
    }

}
