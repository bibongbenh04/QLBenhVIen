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
    }

    public class MedicalRecordDetailsViewModel
    {
        public MedicalRecordViewModel MedicalRecord { get; set; }
        public List<PrescriptionViewModel> Prescriptions { get; set; }
        public List<TestViewModel> Tests { get; set; }
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
