using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffsController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly IDepartmentService _departmentService;
        private readonly IAccountService _accountService;

        public StaffsController(
            IStaffService staffService,
            IDepartmentService departmentService,
            IAccountService accountService)
        {
            _staffService = staffService;
            _departmentService = departmentService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(int departmentId, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var staffList = await _staffService.GetStaffByDepartmentIdAsync(departmentId);
            ViewBag.DepartmentId = departmentId;
            var pagedList = staffList.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        
        public async Task<IActionResult> Create(int departmentId)
        {
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.StaffUsers = await _accountService.GetAvailableStaffUsersAsync();
            return View(new StaffCreateViewModel { DepartmentId = departmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.StaffUsers = await _accountService.GetAvailableStaffUsersAsync();
                return View(model);
            }

            await _staffService.CreateStaffAsync(model);
            return RedirectToAction("Index", new { departmentId = model.DepartmentId });
        }


        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null) return NotFound();
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StaffEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                return View(model);
            }

            await _staffService.UpdateStaffAsync(model);
            return RedirectToAction("Index", new { departmentId = model.DepartmentId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var staffEdit = await _staffService.GetStaffByIdAsync(id);
            if (staffEdit == null) return NotFound();

            var staffView = new StaffViewModels
            {
                Id = staffEdit.Id,
                FullName = staffEdit.FirstName + " " + staffEdit.LastName,
                Gender = staffEdit.Gender,
                PhoneNumber = staffEdit.PhoneNumber,
                Email = staffEdit.Email,
                Position = staffEdit.Position,
                JoiningDate = staffEdit.JoiningDate,
                DepartmentId = staffEdit.DepartmentId
            };

            return View(staffView);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _staffService.DeleteStaffAsync(id);
            return RedirectToAction("Index");
        }
    }
}
