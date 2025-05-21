using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentViewModels>> GetAllDepartmentsAsync();
        Task<DepartmentViewModels> GetDepartmentByIdAsync(int id);
        Task<Department> CreateDepartmentAsync(DepartmentViewModels model, string userId);
        Task UpdateDepartmentAsync(DepartmentViewModels model);
        Task DeleteDepartmentAsync(int id);
    }
}
