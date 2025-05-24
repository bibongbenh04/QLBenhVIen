using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using HospitalManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class StaffService : IStaffService
    {
        private readonly IRepository<Staff> _staffRepo;

        public StaffService(IRepository<Staff> staffRepo)
        {
            _staffRepo = staffRepo;
        }

        public async Task<List<StaffViewModels>> GetAllAsync()
        {
            var staffList = await _staffRepo.GetAsync();
            if (staffList == null) return null;

            return staffList.Select(s => new StaffViewModels
            {
                Id = s.Id,
                FullName = s.FirstName + " " + s.LastName,
                Gender = s.Gender,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                Position = s.Position,
                JoiningDate = s.JoiningDate,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department?.Name
            }).ToList();
        }

        public async Task<List<StaffViewModels>> GetStaffByDepartmentIdAsync(int departmentId)
        {
            var staffList = await _staffRepo.GetAsync(s => s.DepartmentId == departmentId && s.IsActive, includeProperties: "Department");

            return staffList.Select(s => new StaffViewModels
            {
                Id = s.Id,
                FullName = s.FirstName + " " + s.LastName,
                Gender = s.Gender,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                Position = s.Position,
                JoiningDate = s.JoiningDate,
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department?.Name
            }).ToList();
        }


        public async Task CreateStaffAsync(StaffCreateViewModel model) {
            var exists = await _staffRepo.Context().Departments.AnyAsync(d => d.Id == model.DepartmentId);
            if (!exists) throw new Exception($"Phòng ban không tồn tại.");

            var isTaken = await _staffRepo.Context().Staff.AnyAsync(s => s.UserId == model.UserId);
            if (isTaken) throw new Exception("Tài khoản đã được gán cho nhân viên khác.");

            var staff = new Staff {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Position = model.Position,
                JoiningDate = model.JoiningDate,
                DepartmentId = model.DepartmentId,
                UserId = model.UserId,
                IsActive = true
            };

            await _staffRepo.AddAsync(staff);
            await _staffRepo.SaveAsync();
        }



        public async Task<StaffEditViewModel> GetStaffByIdAsync(int id)
        {
            var _staff = await _staffRepo.GetAsync(st => st.Id == id && st.IsActive);
            var staff = _staff.FirstOrDefault();
            if (staff == null) return null;

            return new StaffEditViewModel
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Gender = staff.Gender,
                PhoneNumber = staff.PhoneNumber,
                Email = staff.Email,
                Position = staff.Position,
                JoiningDate = staff.JoiningDate,
                DepartmentId = staff.DepartmentId ?? 0,
                UserId = staff.UserId
            };
        }

        public async Task UpdateStaffAsync(StaffEditViewModel model)
        {
            var staff = await _staffRepo.GetByIdAsync(model.Id);
            if (staff == null) return;

            staff.FirstName = model.FirstName;
            staff.LastName = model.LastName;
            staff.Gender = model.Gender;
            staff.PhoneNumber = model.PhoneNumber;
            staff.Email = model.Email;
            staff.Position = model.Position;
            staff.JoiningDate = model.JoiningDate;
            staff.DepartmentId = model.DepartmentId;

            await _staffRepo.UpdateAsync(staff);
            await _staffRepo.SaveAsync();
        }

        public async Task DeleteStaffAsync(int id)
        {
            var staff = await _staffRepo.GetByIdAsync(id);
            if (staff == null) return;
            // await _staffRepo.DeleteAsync(staff);
            staff.IsActive = false;
            await _staffRepo.SaveAsync();
        }
    }
}
