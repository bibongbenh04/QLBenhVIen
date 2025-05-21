using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HospitalManagement.Models.ViewModels
{
    public class DepartmentViewModels
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Department Name")]
        public string Name { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        // Optional: Include list of doctors if needed in the view
        [ValidateNever]
        public List<DoctorViewModel> Doctors { get; set; }
    }
}
