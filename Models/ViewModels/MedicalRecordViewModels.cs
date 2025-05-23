using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.ViewModels
{
    public class MedicalRecordViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int AppointmentId { get; set; }

        [Required]
        public string Diagnosis { get; set; }

        public string Treatment { get; set; }
        public string Notes { get; set; }

        [Display(Name = "Record Date")]
        public DateTime RecordDate { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AdmissionDate { get; set; }
        public bool HasBill { get; set; } = false;

    }

    public class MedicalRecordDetailsViewModel
    {
        public MedicalRecordViewModel MedicalRecord { get; set; } = new();
        public List<PrescriptionViewModel> Prescriptions { get; set; } = new();
        public List<TestViewModel> Tests { get; set; } = new();
    }

    public class MedicalRecordCreateViewModel
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }

        [Required]
        public string Diagnosis { get; set; }

        public string Treatment { get; set; }
        public string Notes { get; set; }
    }
}
