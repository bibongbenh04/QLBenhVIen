using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using static HospitalManagement.Models.ViewModels.StaffViewModels;

namespace HospitalManagement.Services.Interfaces
{
    public interface IStaffPayrollService
    {
        Task<IEnumerable<StaffPayrollViewModel>> GetAllByStaffIdAsync(int staffId);
        Task<IEnumerable<StaffPayrollViewModel>> GetByStaffIdAsync(int staffId);
        Task<StaffPayroll> GetByIdAsync(int id);
        Task CreateAsync(StaffPayroll payroll);
        Task UpdateAsync(StaffPayrollViewModel model);
        Task DeleteAsync(int id);
    }

}
