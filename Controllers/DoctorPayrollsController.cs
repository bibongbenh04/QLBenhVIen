using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
{
    [Authorize(Policy = "DoctorPayrollsControllerAccess")]
    public class DoctorPayrollsController : Controller
    {
        private readonly IDoctorPayrollService _doctorPayrollService;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;

        public DoctorPayrollsController(IDoctorPayrollService doctorPayrollService, IDoctorService doctorService)
        {
            _doctorPayrollService = doctorPayrollService;
            _doctorService = doctorService;
        }

        public async Task<IActionResult> Index(int doctorId)
        {
            var payrolls = await _doctorPayrollService.GetAllByDoctorIdAsync(doctorId);
            if (payrolls == null)
            {
                ViewBag.Error = "Chưa hóa đơn nào";
                return View("ListDoctor");
            }
            return View(payrolls);
        }

        [HttpGet]
        public async Task<IActionResult> ListDoctor(int? page, string? keyword)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var doctors = await _doctorService.GetAllDoctorsAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                doctors = doctors.Where(u =>
                    !string.IsNullOrEmpty(u.FullName) && u.FullName.ToLower().Contains(keyword) || 
                    !string.IsNullOrEmpty(u.Specialization) && u.Specialization.ToLower().Contains(keyword)
                );
            }

            ViewBag.Keyword = keyword;


            var pagedList = doctors.ToPagedList(pageNumber, pageSize);

            return View("ListDoctor", pagedList);
        }

        public async Task<IActionResult> Create(int doctorId)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (doctor == null) return NotFound();

            var now = DateTime.Now;
            var appointments = await _doctorPayrollService.GetAsync(
                a => a.DoctorId == doctor.Id &&
                    a.AppointmentDate.Month == now.Month &&
                    a.AppointmentDate.Year == now.Year &&
                    a.IsPaidByPatient == true &&
                    a.IsPaidToDoctor == false
            );

            var model = new DoctorPayrollViewModel
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.FirstName + " " + doctor.LastName,
                Month = now.Month,
                Year = now.Year,
                TotalAppointments = appointments.Count()
            };

            return View(model); 
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorPayrollViewModel model)
        {

            if (!ModelState.IsValid)
            {
                // ViewBag.Doctors = await _doctorService.GetAllDoctorsAsync();
                return View(model);
            }

            try
            {
                var payroll = new DoctorPayroll
                {
                    DoctorId = model.DoctorId,
                    Month = model.Month,
                    Year = model.Year,
                    TotalAppointments = model.TotalAppointments,
                    BaseSalary = model.BaseSalary,
                    Bonus = model.Bonus,
                    TotalSalary = model.TotalSalary,
                    Notes = model.Notes,
                    Status = model.Status,
                    CreatedAt = DateTime.Now
                };

                await _doctorPayrollService.CreatePayrollWithAutoAppointmentsAsync(payroll);
                return RedirectToAction("Index", new { doctorId = model.DoctorId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Create", new { doctorId = model.DoctorId });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var payroll = await _doctorPayrollService.GetByIdAsync(id);
            if (payroll == null) return NotFound();

            var model = new DoctorPayrollViewModel
            {
                Id = payroll.Id,
                DoctorId = payroll.DoctorId,
                DoctorName = payroll.Doctor.FirstName + " " + payroll.Doctor.LastName,
                Month = payroll.Month,
                Year = payroll.Year,
                TotalAppointments = payroll.TotalAppointments,
                BaseSalary = payroll.BaseSalary,
                Bonus = payroll.Bonus,
                TotalSalary = payroll.TotalSalary,
                Notes = payroll.Notes,
                Status = payroll.Status
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DoctorPayrollViewModel model)
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

            await _doctorPayrollService.UpdateAsync(model);
            return RedirectToAction("Index", new { doctorId = model.DoctorId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var payroll = await _doctorPayrollService.GetByIdAsync(id);
            if (payroll == null) return NotFound();

            var model = new DoctorPayrollViewModel
            {
                Id = payroll.Id,
                DoctorId = payroll.DoctorId,
                DoctorName = payroll.Doctor?.FirstName + " " + payroll.Doctor?.LastName,
                Month = payroll.Month,
                Year = payroll.Year,
                TotalAppointments = payroll.TotalAppointments,
                BaseSalary = payroll.BaseSalary,
                Bonus = payroll.Bonus,
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
            var payroll = await _doctorPayrollService.GetByIdAsync(id);
            if (payroll == null) return NotFound();

            await _doctorPayrollService.DeleteAsync(id);
            return RedirectToAction("Index", new { doctorId = payroll.DoctorId });
        }
    }

}