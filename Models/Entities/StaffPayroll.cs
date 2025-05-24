using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class StaffPayroll
    {
        public int Id { get; set; }

        [Required]
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        public int Year { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BaseSalary { get; set; }

        public decimal Allowance { get; set; }

        public decimal Deduction { get; set; }

        public decimal TotalSalary { get; set; }

        [StringLength(250)]
        public string Notes { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }


}
