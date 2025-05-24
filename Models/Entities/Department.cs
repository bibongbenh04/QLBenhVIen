using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Navigation properties
        public ICollection<Doctor> Doctors { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
