using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.ViewModels
{
    public class AppointmentViewModels
    {
        public int Id { get; set; }

        // [Required]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }
        public string PatientName { get; set; }

        // [Required]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Display(Name = "Appointment Time")]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [Display(Name = "Reason for Visit")]
        public string Reason { get; set; }

        public string Notes { get; set; } = "";

        public string Status { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        public List<PatientViewModel> Patients { get; set; } = new();
        public List<DoctorViewModel> Doctors { get; set; } = new();
        public bool HasMedicalRecord { get; set; } 
        

    }

    public class AppointmentCreateViewModel
    {
        [Required]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Display(Name = "Appointment Time")]
        public string AppointmentTime { get; set; }

        [Display(Name = "Reason for Visit")]
        public string Reason { get; set; }
        public string Status { get; set; } = "Đang chờ"; // optional user control
        public string Notes { get; set; } = "";

        // For dropdown lists
        public List<PatientViewModel>? Patients { get; set; }
        public List<DoctorViewModel>? Doctors { get; set; }
        public List<string>? AvailableTimeSlots { get; set; }
    }
}
