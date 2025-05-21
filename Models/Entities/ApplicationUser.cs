using Microsoft.AspNetCore.Identity;
using System;

namespace HospitalManagement.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; } // Change to DateTime for compatibility
        public DateTime? LastLogin { get; set; } // Change to DateTime for compatibility
        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<Patient> Patients { get; set; }

        public Doctor Doctor { get; set; }
        public Staff Staff { get; set; }
    }
}
