using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(50)]
        public string EmergencyContactName { get; set; }

        [StringLength(15)]
        public string EmergencyContactPhone { get; set; }

        [StringLength(50)]
        public string BloodGroup { get; set; }

        public bool HasInsurance { get; set; }

        [StringLength(50)]
        public string InsuranceProvider { get; set; } = String.Empty;

        [StringLength(50)]
        public string InsurancePolicyNumber { get; set; } = String.Empty;

        // Navigation properties
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public ICollection<Bill> Bills { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
