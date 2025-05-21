using HospitalManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.ViewModels
{
    public class DoctorViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Gender { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Specialization { get; set; }

        public string Qualifications { get; set; }

        public string Biography { get; set; }

        [Display(Name = "Consultation Fee")]
        public decimal ConsultationFee { get; set; }

        // [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        [Required]
        public string UserId { get; set; }
    }

    public class DoctorDetailsViewModel
    {
        public DoctorViewModel Doctor { get; set; }
        public List<DoctorScheduleViewModel> Schedules { get; set; }
        public List<AppointmentViewModels> Appointments { get; set; }
    }

    public class DoctorScheduleViewModel
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        public int MaxAppointments { get; set; }

        public bool IsAvailable { get; set; }

        // [BindNever]
        // public string DoctorName { get; set; }
    }

}
