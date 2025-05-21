using System;
using System.ComponentModel.DataAnnotations;
using HospitalManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HospitalManagement.Models.ViewModels
{
    public class StaffViewModels
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public DateTime JoiningDate { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }

    public class StaffCreateViewModel
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [Required] public string Position { get; set; }
        [Required] public DateTime JoiningDate { get; set; }
        public int DepartmentId { get; set; }

        public string UserId { get; set; }
    }

    public class StaffEditViewModel : StaffCreateViewModel
    {
        public int Id { get; set; }
    }


}