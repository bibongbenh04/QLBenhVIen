using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class Test
    {
        public int Id { get; set; }

        public int MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }

        [Required]
        [StringLength(100)]
        public string TestName { get; set; }

        [StringLength(500)]
        public string TestDescription { get; set; }

        public DateTime TestDate { get; set; }

        [StringLength(1000)]
        public string Results { get; set; }

        [StringLength(20)]
        public string Status { get; set; } // Ordered, Completed, Cancelled

        public decimal Cost { get; set; }
    }
}
