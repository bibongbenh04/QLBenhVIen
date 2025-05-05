using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }

        [StringLength(20)]
        public string Status { get; set; } // Scheduled, Completed, Cancelled, No-Show

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public MedicalRecord MedicalRecord { get; set; }
    }
}
