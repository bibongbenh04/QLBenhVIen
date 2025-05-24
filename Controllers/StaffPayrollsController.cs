using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList.Extensions;
using HospitalManagement.Models.Entities;

namespace HospitalManagement.Controllers
{
    public class StaffPayrollsController : Controller
    {
        private readonly IStaffPayrollService _staffPayrollService;
        private readonly IStaffService _staffService;

        public StaffPayrollsController(
            IStaffPayrollService staffPayrollService,
            IStaffService staffService)
        {
            _staffPayrollService = staffPayrollService;
            _staffService = staffService;
        }

        public async Task<IActionResult> Index(int staffId)
        {
            var list = await _staffPayrollService.GetAllByStaffIdAsync(staffId);
            return View(list);
        }

        public async Task<IActionResult> ListStaff(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var staffList = await _staffService.GetAllAsync();
            var pagedList = staffList.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        public async Task<IActionResult> Create(int staffId)
        {
            var staff = await _staffService.GetStaffByIdAsync(staffId);
            if (staff == null) return NotFound();

            Console.WriteLine("OKKKKKKKKKKKKKKK " + staff.FirstName + " " + staff.LastName);

            var model = new StaffPayrollViewModel
            {
                StaffId = staff.Id,
                StaffName = staff.FirstName + " " + staff.LastName,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffPayrollViewModel model)
        {
            ModelState.Remove(nameof(model.StaffName));

            if (!ModelState.IsValid)
            {
                var staff = await _staffService.GetStaffByIdAsync(model.StaffId);
                model.StaffName = staff?.FirstName + " " + staff?.LastName;
                return View(model);
            }

            try
            {
                var existing = await _staffPayrollService.GetAllByStaffIdAsync(model.StaffId);
                bool exists = existing.Any(p => p.Month == model.Month && p.Year == model.Year);

                if (exists)
                {
                    TempData["ErrorMessage"] = $"Bảng lương cho tháng {model.Month}/{model.Year} đã tồn tại.";
                    return View(model);
                }

                var payroll = new StaffPayroll
                {
                    StaffId = model.StaffId,
                    Month = model.Month,
                    Year = model.Year,
                    BaseSalary = model.BaseSalary,
                    Allowance = model.Allowance,
                    Deduction = model.Deduction,
                    Notes = model.Notes,
                    Status = model.Status,
                    CreatedAt = DateTime.Now
                };

                await _staffPayrollService.CreateAsync(payroll);
                return RedirectToAction("Index", new { staffId = model.StaffId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Create", new { staffId = model.StaffId });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var payroll = await _staffPayrollService.GetByIdAsync(id);
            if (payroll == null) return NotFound();

            var model = new StaffPayrollViewModel
            {
                Id = payroll.Id,
                StaffId = payroll.StaffId,
                StaffName = payroll.Staff?.FirstName + " " + payroll.Staff?.LastName,
                Month = payroll.Month,
                Year = payroll.Year,
                BaseSalary = payroll.BaseSalary,
                Allowance = payroll.Allowance,
                Deduction = payroll.Deduction,
                TotalSalary = payroll.TotalSalary,
                Notes = payroll.Notes,
                Status = payroll.Status
            };
            

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StaffPayrollViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var err in ModelState)
                {
                    Console.WriteLine($"[MODELSTATE] Key: {err.Key}");
                    foreach (var e in err.Value.Errors)
                    {
                        Console.WriteLine($"   => Error: {e.ErrorMessage}");
                    }
                }
            }
            if (!ModelState.IsValid) return View(model);
            await _staffPayrollService.UpdateAsync(model);
            return RedirectToAction("Index", new { staffId = model.StaffId });

        }

        public async Task<IActionResult> Delete(int id)
        {
            var payroll = await _staffPayrollService.GetByIdAsync(id);
            if (payroll == null) return NotFound();

            var model = new StaffPayrollViewModel
            {
                Id = payroll.Id,
                StaffId = payroll.StaffId,
                StaffName = payroll.Staff?.FirstName + " " + payroll.Staff?.LastName,
                Month = payroll.Month,
                Year = payroll.Year,
                BaseSalary = payroll.BaseSalary,
                Allowance = payroll.Allowance,
                Deduction = payroll.Deduction,
                TotalSalary = payroll.TotalSalary,
                Notes = payroll.Notes,
                Status = payroll.Status
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _staffPayrollService.GetByIdAsync(id);
            if (payroll == null) return NotFound();
            await _staffPayrollService.DeleteAsync(id);
            return RedirectToAction("Index", new { staffId = payroll.StaffId });
        }
    }
}
