using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.ViewModels
{
    public class MedicationViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string GenericName { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string DosageForm { get; set; }
        public string Strength { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }
}
