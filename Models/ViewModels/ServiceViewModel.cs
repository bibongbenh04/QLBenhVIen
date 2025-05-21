using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models.ViewModels
{
    public class ServiceViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, 100000000)]
        public decimal Price { get; set; }
    }
}
