using HospitalManagement.Core.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HospitalManagement.Web.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IPatientService patientService,
            IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return View(appointments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        public async Task<IActionResult> Create()
        {
            var model = new AppointmentCreateViewModel
            {
                AppointmentDate = DateTime.Now.Date.AddDays(1),
                Patients = (await _patientService.GetAllPatientsAsync()).ToList(),
                Doctors = (await _doctorService.GetAllDoctorsAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _appointmentService.CreateAppointmentAsync(model);
                return RedirectToAction(nameof(Index));
            }

            model.Patients = (await _patientService.GetAllPatientsAsync()).ToList();
            model.Doctors = (await _doctorService.GetAllDoctorsAsync()).ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _appointmentService.UpdateAppointmentAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,Staff,Doctor")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff,Doctor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _appointmentService.DeleteAppointmentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(int doctorId, DateTime date)
        {
            var timeSlots = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId, date);
            return Json(timeSlots);
        }
    }
}
