using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class MedicalRecord
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public Bill? Bill { get; set; } 

        [Required]
        [StringLength(500)]
        public string Diagnosis { get; set; }

        [StringLength(1000)]
        public string Treatment { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        public DateTime RecordDate { get; set; }

        // Navigation properties
        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<Test> Tests { get; set; }

    }
}
