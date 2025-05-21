using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class DoctorSchedule
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int MaxAppointments { get; set; }

        public bool IsAvailable { get; set; }
    }
}
