using System;
using System.ComponentModel.DataAnnotations;
using HospitalManagement.Models.Entities;

namespace HospitalManagement.Models.ViewModels
{
    public class DoctorPayrollViewModel
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = "";
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalAppointments { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal TotalSalary { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } = "Pending";
    }
}