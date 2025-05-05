using HospitalManagement.Models.Entities;
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

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }

    public class DoctorDetailsViewModel
    {
        public DoctorViewModel Doctor { get; set; }
        public List<DoctorScheduleViewModel> Schedules { get; set; }
        public List<AppointmentViewModel> Appointments { get; set; }
    }

    public class DoctorScheduleViewModel
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int MaxAppointments { get; set; }
        public bool IsAvailable { get; set; }
    }
}
