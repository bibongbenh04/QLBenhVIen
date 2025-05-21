using HospitalManagement.Services.Interfaces;
using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentService(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<DepartmentViewModels>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return departments.Select(d => new DepartmentViewModels
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            });
        }

        public async Task<DepartmentViewModels> GetDepartmentByIdAsync(int id)
        {
            var d = await _departmentRepository.GetByIdAsync(id);
            return new DepartmentViewModels
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            };
        }

        public async Task<Department> CreateDepartmentAsync(DepartmentViewModels model, string userId)
        {
            var dept = new Department
            {
                Name = model.Name,
                Description = model.Description
            };
            await _departmentRepository.AddAsync(dept);
            await _departmentRepository.SaveAsync();
            return dept;
        }

        public async Task UpdateDepartmentAsync(DepartmentViewModels model)
        {
            var dept = await _departmentRepository.GetByIdAsync(model.Id);
            if (dept == null) return;

            dept.Name = model.Name;
            dept.Description = model.Description;
            await _departmentRepository.UpdateAsync(dept);
            await _departmentRepository.SaveAsync();
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var dept = await _departmentRepository.GetByIdAsync(id);
            await _departmentRepository.DeleteAsync(dept);
            await _departmentRepository.SaveAsync();
        }
    }
}