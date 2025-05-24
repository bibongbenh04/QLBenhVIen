using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HospitalManagement.Models.Entities
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, 100000000)]
        public decimal Price { get; set; }

        public ICollection<Test> Tests { get; set; }
        public bool IsActive { get; set; } = true;
    }
}