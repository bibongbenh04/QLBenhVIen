using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Core.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync();
        Task<DoctorViewModel> GetDoctorByIdAsync(int id);
        Task<DoctorDetailsViewModel> GetDoctorDetailsAsync(int id);
        Task<Doctor> CreateDoctorAsync(DoctorViewModel model, string userId);
        Task UpdateDoctorAsync(DoctorViewModel model);
        Task DeleteDoctorAsync(int id);
        Task<IEnumerable<DoctorScheduleViewModel>> GetDoctorSchedulesAsync(int doctorId);
        Task UpdateDoctorScheduleAsync(DoctorScheduleViewModel model);
    }
}
