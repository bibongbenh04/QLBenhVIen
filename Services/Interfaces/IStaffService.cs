using HospitalManagement.Models.ViewModels;
using static HospitalManagement.Models.ViewModels.StaffViewModels;

namespace HospitalManagement.Services.Interfaces
{
    public interface IStaffService
    {
        Task<List<StaffViewModels>> GetAllAsync();
        Task<List<StaffViewModels>> GetStaffByDepartmentIdAsync(int departmentId);
        Task CreateStaffAsync(StaffCreateViewModel model);

        Task<StaffEditViewModel> GetStaffByIdAsync(int id);

        Task UpdateStaffAsync(StaffEditViewModel model);

        Task DeleteStaffAsync(int id);
    }
}
