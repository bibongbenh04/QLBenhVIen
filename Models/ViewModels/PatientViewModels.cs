using HospitalManagement.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.ViewModels
{
    public class PatientViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }

        [Display(Name = "Emergency Contact Name")]
        public string EmergencyContactName { get; set; }

        [Display(Name = "Emergency Contact Phone")]
        public string EmergencyContactPhone { get; set; }

        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }

        [Display(Name = "Has Insurance")]
        public bool HasInsurance { get; set; }

        [Display(Name = "Insurance Provider")]
        public string InsuranceProvider { get; set; }

        [Display(Name = "Insurance Policy Number")]
        public string InsurancePolicyNumber { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public bool HasAppointmentToday { get; set; } = false;
    }

    public class PatientDetailsViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public PatientViewModel Patient { get; set; }
        public List<AppointmentViewModels> Appointments { get; set; }
        public List<MedicalRecordViewModel> MedicalRecords { get; set; }
        public List<BillViewModel> Bills { get; set; }
    }
}
