using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class Prescription
    {
        public int Id { get; set; }

        public int MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public DateTime PrescriptionDate { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        // Navigation properties
        public ICollection<PrescriptionItem> PrescriptionItems { get; set; }
    }

    public class PrescriptionItem
    {
        public int Id { get; set; }

        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }

        public int MedicationId { get; set; }
        public Medication Medication { get; set; }

        [Required]
        [StringLength(100)]
        public string Dosage { get; set; }

        [Required]
        [StringLength(200)]
        public string Instructions { get; set; }

        public int Quantity { get; set; }

        public int DurationDays { get; set; }
    }
}
