using System;
using System.ComponentModel.DataAnnotations;
using HospitalManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HospitalManagement.Models.ViewModels
{
    public class TestCreateViewModel
    {
        [Required]
        public int MedicalRecordId { get; set; }

        [Required]
        [Display(Name = "Dịch vụ")]
        public int ServiceId { get; set; }
    }

    public class TestViewModel
    {
        public int Id { get; set; }

        [Required]
        public int MedicalRecordId { get; set; }

        [ValidateNever] // tránh lỗi binding vòng lặp
        public MedicalRecord MedicalRecord { get; set; }

        [Required(ErrorMessage = "Bạn phải chọn dịch vụ.")]
        [Display(Name = "Dịch vụ")]
        public int ServiceId { get; set; }

        [ValidateNever]
        public Service Service { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Results { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime TestDate { get; set; }
    }

}