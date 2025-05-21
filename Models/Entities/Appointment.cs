using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }

        public string Notes { get; set; } = "";

        [StringLength(20)]
        public string Status { get; set; } = "No-Show";// Scheduled, Completed, Cancelled, No-Show

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public MedicalRecord MedicalRecord { get; set; }
    }
}
