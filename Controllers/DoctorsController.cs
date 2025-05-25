using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HospitalManagement.Controllers
{
    [Authorize(Policy = "DoctorsControllerAccess")]
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IDepartmentService _departmentService;
        private readonly IAccountService _accountService;

        public DoctorsController(
            IDoctorService doctorService,
            IDepartmentService departmentService,
            IAccountService accountService)
        {
            _doctorService = doctorService;
            _departmentService = departmentService;
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var doctors = await _doctorService.GetAllDoctorsAsync();
            var pagedList = doctors.ToPagedList(pageNumber, pageSize);

            return View("Index", pagedList);
        }

        // public async Task<IActionResult> Index(string? keyword, int? page)
        // {
        //     int pageNumber = page ?? 1;
        //     int pageSize = 5;

        //     var doctors = await _doctorService.GetAllDoctorsAsync();

        //     if (!string.IsNullOrWhiteSpace(keyword))
        //         doctors = doctors.Where(d => d.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

        //     var pagedList = doctors.ToPagedList(pageNumber, pageSize);
        //     ViewBag.Keyword = keyword;

        //     return View(pagedList);
        // }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("Doctors/Index/{departmentId}")]
        public async Task<IActionResult> Index(int departmentId, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var doctors = await _doctorService.GetDoctorsByDepartmentIdAsync(departmentId);
            var pagedList = doctors.ToPagedList(pageNumber, pageSize);
            return View("Index", pagedList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctorDetails = await _doctorService.GetDoctorDetailsAsync(id);
            if (doctorDetails == null) return NotFound();
            return View("Details", doctorDetails);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.DoctorUsers = await _accountService.GetAvailableDoctorUsersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(DoctorViewModel model)
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
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.DoctorUsers = await _accountService.GetAvailableDoctorUsersAsync();
                return View(model);
            }

            var user = await _accountService.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.EmailError = "Email chưa được đăng ký tài khoản.";
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.DoctorUsers = await _accountService.GetAvailableDoctorUsersAsync();
                return View(model);
            }


            try
            {
                model.UserId = user.Id;

                await _doctorService.CreateDoctorAsync(model, model.UserId);

                // Gán quyền Doctor nếu chưa có
                var roles = await _accountService.GetRolesAsync(user.Id);
                if (!roles.Contains("Doctor"))
                {
                    await _accountService.AssignRoleAsync(user.Id, "Doctor");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.DoctorUsers = await _accountService.GetAvailableDoctorUsersAsync();
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();

            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(DoctorViewModel model)
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
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                return View(model);
            }

            await _doctorService.UpdateDoctorAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();
            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _doctorService.DeleteDoctorAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Schedule(int id)
        {
            var schedules = await _doctorService.GetDoctorSchedulesAsync(id);
            ViewBag.DoctorId = id;
            return View(schedules);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateSchedule(DoctorScheduleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _doctorService.UpdateDoctorScheduleAsync(model);
            return RedirectToAction(nameof(Schedule), new { id = model.DoctorId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult AddSchedule(int id)
        {
            return View(new DoctorScheduleViewModel { DoctorId = id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSchedule(DoctorScheduleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _doctorService.AddDoctorScheduleAsync(model);
            return RedirectToAction("Details", new { id = model.DoctorId });
        }

        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> EditSchedule(int id)
        {
            var schedule = await _doctorService.GetScheduleByIdAsync(id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> EditSchedule(DoctorScheduleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _doctorService.UpdateDoctorScheduleAsync(model);
            return RedirectToAction("Details", new { id = model.DoctorId });
        }

        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _doctorService.GetScheduleByIdAsync(id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        [HttpPost, ActionName("DeleteSchedule")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> DeleteScheduleConfirmed(int id)
        {
            var doctorId = await _doctorService.DeleteDoctorScheduleAsync(id);
            return RedirectToAction("Details", new { id = doctorId });
        }

        [HttpGet]
        public async Task<IActionResult> SearchAvailable(string term, DateTime date, TimeSpan time)
        {
            var doctors = await _doctorService.GetAvailableDoctorsAsync(date, time);
            var matched = doctors
                .Where(d => (d.FirstName + " " + d.LastName).Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(d => new {
                    id = d.Id,
                    fullName = d.FirstName + " " + d.LastName,
                    specialization = d.Specialization
                });
            return Json(matched);
        }

    }
}
