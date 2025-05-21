using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class Test
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public DateTime TestDate { get; set; } = DateTime.Now;
        public string Results { get; set; }
        public string Status { get; set; }

        
    }

}
