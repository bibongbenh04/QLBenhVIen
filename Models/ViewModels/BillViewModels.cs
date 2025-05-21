using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HospitalManagement.Models.ViewModels
{
    public class BillViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime BillDate { get; set; }
        public decimal InsuranceCoverage { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        [ValidateNever]
        public List<BillItemViewModel> Items { get; set; }
    }

    public class BillItemViewModel
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class BillCreateViewModel
    {
        [Required]
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public bool HasInsurance { get; set; }
        public decimal InsuranceCoverage { get; set; }
        public string InsuranceProvider { get; set; }
        public string InsurancePolicyNumber { get; set; }
        public List<BillItemCreateViewModel> Items { get; set; }
        [ValidateNever]
        public List<PatientViewModel> Patients { get; set; }
    }

    public class BillItemCreateViewModel
    {
        [Required]
        public string ItemName { get; set; }

        [Required]
        public string ItemType { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }
    }
}
