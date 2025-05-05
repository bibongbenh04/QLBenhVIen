using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class Bill
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public DateTime BillDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal DueAmount { get; set; }

        public decimal InsuranceCoverage { get; set; }

        [StringLength(20)]
        public string PaymentStatus { get; set; } // Paid, Partial, Pending

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        // Navigation properties
        public ICollection<BillItem> BillItems { get; set; }
    }

    public class BillItem
    {
        public int Id { get; set; }

        public int BillId { get; set; }
        public Bill Bill { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }

        [StringLength(20)]
        public string ItemType { get; set; } // Consultation, Medication, Test, Procedure

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Subtotal { get; set; }
    }
}
