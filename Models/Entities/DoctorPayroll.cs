using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class DoctorPayroll
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        public int Year { get; set; }

        public int TotalAppointments { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BaseSalary { get; set; }

        public decimal Bonus { get; set; }

        public decimal TotalSalary { get; set; }

        [StringLength(250)]
        public string Notes { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Paid, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }


}
