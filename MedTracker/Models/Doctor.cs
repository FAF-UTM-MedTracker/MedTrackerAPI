using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedTracker.Models
{
    [Table("Doctor")]
    public class Doctor
    {
        [Key]
        [ForeignKey("User")]
        public int IdUser { get; set; }        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public User User { get; set; }
    }
}
