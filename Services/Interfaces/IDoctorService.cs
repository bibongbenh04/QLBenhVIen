using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync();
        Task<DoctorViewModel> GetDoctorByIdAsync(int id);
        Task<DoctorDetailsViewModel> GetDoctorDetailsAsync(int id);
        Task<List<DoctorViewModel>> GetDoctorsByDepartmentIdAsync(int departmentId);
        Task<Doctor> CreateDoctorAsync(DoctorViewModel model, string userId);
        Task UpdateDoctorAsync(DoctorViewModel model);
        Task DeleteDoctorAsync(int id);
        Task AddDoctorScheduleAsync(DoctorScheduleViewModel model);
        Task<DoctorScheduleViewModel> GetScheduleByIdAsync(int id);
        Task<int> DeleteDoctorScheduleAsync(int id);
        Task<IEnumerable<DoctorScheduleViewModel>> GetDoctorSchedulesAsync(int doctorId);
        Task UpdateDoctorScheduleAsync(DoctorScheduleViewModel model);
        Task<List<DoctorViewModel>> GetAvailableDoctorsAsync(DateTime date, TimeSpan time);
        Task<IEnumerable<DoctorViewModel>> GetPaginatedDoctorsAsync(int pageNumber, int pageSize);

    }
}
