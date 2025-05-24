using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using HospitalManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class StaffPayrollService : IStaffPayrollService
    {
        private readonly IRepository<StaffPayroll> _payrollRepo;
        private readonly IRepository<Staff> _staffRepo;

        public StaffPayrollService(IRepository<StaffPayroll> payrollRepo, IRepository<Staff> staffRepo)
        {
            _payrollRepo = payrollRepo;
            _staffRepo = staffRepo;
        }

        public async Task<IEnumerable<StaffPayrollViewModel>> GetAllByStaffIdAsync(int staffId)
        {
            var list = await _payrollRepo.GetAsync(
                p => p.StaffId == staffId,
                includeProperties: "Staff");

            return list.Select(p => new StaffPayrollViewModel
            {
                Id = p.Id,
                StaffId = p.StaffId,
                StaffName = p.Staff.FirstName + " " + p.Staff.LastName,
                Month = p.Month,
                Year = p.Year,
                BaseSalary = p.BaseSalary,
                Allowance = p.Allowance,
                Deduction = p.Deduction,
                TotalSalary = p.TotalSalary,
                Notes = p.Notes,
                Status = p.Status
            });
        }

        public async Task<StaffPayroll> GetByIdAsync(int id)
        {
            return (await _payrollRepo.GetAsync(p => p.Id == id, includeProperties: "Staff"))?.FirstOrDefault();
        }

        public async Task CreateAsync(StaffPayroll payroll)
        {
            var exists = await _payrollRepo.GetFirstOrDefaultAsync(
                    p => p.StaffId == payroll.StaffId &&
                    p.Month == payroll.Month &&
                    p.Year == payroll.Year
            );

            if (exists != null)
                throw new Exception($"Bảng lương đã tồn tại cho tháng {exists.Month}/{exists.Year}.");

            payroll.TotalSalary = payroll.BaseSalary + payroll.Allowance - payroll.Deduction;
            payroll.CreatedAt = DateTime.Now;
            await _payrollRepo.AddAsync(payroll);
            await _payrollRepo.SaveAsync();
        }

        public async Task UpdateAsync(StaffPayroll payroll)
        {
            payroll.TotalSalary = payroll.BaseSalary + payroll.Allowance - payroll.Deduction;
            await _payrollRepo.UpdateAsync(payroll);
            await _payrollRepo.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _payrollRepo.GetByIdAsync(id);
            if (entity != null)
            {
                await _payrollRepo.DeleteAsync(entity);
                await _payrollRepo.SaveAsync();
            }
        }

        public Task<IEnumerable<StaffPayrollViewModel>> GetByStaffIdAsync(int staffId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(StaffPayrollViewModel model)
        {
            var entity = await _payrollRepo.GetByIdAsync(model.Id);
            if (entity == null) return;

            entity.BaseSalary = model.BaseSalary;
            entity.Allowance = model.Allowance;
            entity.Deduction = model.Deduction;
            entity.Notes = model.Notes;
            entity.Status = model.Status;
            entity.TotalSalary = model.BaseSalary + model.Allowance - model.Deduction;

            await _payrollRepo.UpdateAsync(entity);
            await _payrollRepo.SaveAsync();
        }

    }
}
