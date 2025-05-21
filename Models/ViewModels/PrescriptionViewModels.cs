using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.ViewModels
{
    public class PrescriptionViewModel
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string Notes { get; set; }
        public List<PrescriptionItemViewModel> Items { get; set; }
    }

    public class PrescriptionItemViewModel
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public int MedicationId { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }
        public int Quantity { get; set; }
        public int DurationDays { get; set; }
    }

    public class PrescriptionCreateViewModel
    {
        public int MedicalRecordId { get; set; }
        public int DoctorId { get; set; }
        public string Notes { get; set; }
        public List<PrescriptionItemCreateViewModel> Items { get; set; } = new();
        public List<MedicationViewModel> AvailableMedications { get; set; } = new();
    }

    public class PrescriptionItemCreateViewModel
    {
        [Required]
        public int MedicationId { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public string Instructions { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int DurationDays { get; set; }
    }
}
