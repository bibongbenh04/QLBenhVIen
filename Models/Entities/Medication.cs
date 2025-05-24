using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.Entities
{
    public class Medication
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string GenericName { get; set; }

        [StringLength(100)]
        public string Brand { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(100)]
        public string DosageForm { get; set; } // Tablet, Capsule, Syrup, etc.

        [StringLength(100)]
        public string Strength { get; set; }

        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        public int ReorderLevel { get; set; }

        // Navigation properties
        public ICollection<PrescriptionItem> PrescriptionItems { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
