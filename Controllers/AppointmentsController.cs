using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
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

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var list = await _appointmentService.GetAllAppointmentsAsync();
            var pagedList = list.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        public async Task<IActionResult> ListPatient(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var patients = await _patientService.GetAllPatientsAsync();

            var today = DateTime.Today;

            var viewModels = new List<PatientViewModel>();
            foreach (var p in patients)
            {
                bool hasToday = await _appointmentService.HasExistingAppointment(p.Id, today);
                viewModels.Add(new PatientViewModel
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    Address = p.Address,
                    PhoneNumber = p.PhoneNumber,
                    HasAppointmentToday = hasToday
                });
            }

            return View(viewModels.ToPagedList(pageNumber, pageSize));
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

        public async Task<IActionResult> Create(int patientId)
        {
            var model = new AppointmentCreateViewModel
            {
                PatientId = patientId,
                AppointmentDate = DateTime.Today,
                Status = "Waiting",
                Patients = (await _patientService.GetAllPatientsAsync()).ToList(),
                Doctors = (await _doctorService.GetAllDoctorsAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Patients = (await _patientService.GetAllPatientsAsync()).ToList();
                model.Doctors = (await _doctorService.GetAllDoctorsAsync()).ToList();
                return View(model);
            }

            var exists = await _appointmentService.HasExistingAppointment(model.PatientId, model.AppointmentDate);
            if (exists)
            {
                TempData["DuplicateAppointment"] = true;
                return RedirectToAction("ListPatient");
            }

            var availableDoctors = await _doctorService.GetAvailableDoctorsAsync(model.AppointmentDate, TimeSpan.Parse(model.AppointmentTime));
            var selectedDoctor = availableDoctors.FirstOrDefault(d => d.Id == model.DoctorId);
            if (selectedDoctor == null)
            {
                TempData["DoctorUnavailable"] = true;
                return RedirectToAction("Create");
            }

            await _appointmentService.CreateAppointmentAsync(model);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            appointment.Patients = (await _patientService.GetAllPatientsAsync()).ToList();
            appointment.Doctors = (await _doctorService.GetAllDoctorsAsync()).ToList();
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentViewModels model)
        {
            ModelState.Remove("DoctorName");
            ModelState.Remove("PatientName");

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
            if (ModelState.IsValid)
            {
                await _appointmentService.UpdateAppointmentAsync(model);
                return RedirectToAction(nameof(Index));
            }

            model.Patients = (await _patientService.GetAllPatientsAsync()).ToList();
            model.Doctors = (await _doctorService.GetAllDoctorsAsync()).ToList();
            
            return View(model);
        }

        [Authorize(Roles = "Admin,Staff,Doctor")]
        public async Task<IActionResult> Delete(int id)
        {
            // if (!ModelState.IsValid)
            // {
            //     foreach (var err in ModelState)
            //     {
            //         Console.WriteLine($"[MODELSTATE] Key: {err.Key}");
            //         foreach (var e in err.Value.Errors)
            //         {
            //             Console.WriteLine($"   => Error: {e.ErrorMessage}");
            //         }
            //     }
            // }
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
