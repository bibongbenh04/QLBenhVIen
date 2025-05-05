using HospitalManagement.Core.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HospitalManagement.Web.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class MedicalRecordsController : Controller
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IAppointmentService _appointmentService;

        public MedicalRecordsController(
            IMedicalRecordService medicalRecordService,
            IAppointmentService appointmentService)
        {
            _medicalRecordService = medicalRecordService;
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var medicalRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
            return View(medicalRecords);
        }

        public async Task<IActionResult> Details(int id)
        {
            var medicalRecord = await _medicalRecordService.GetMedicalRecordDetailsAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }
            return View(medicalRecord);
        }

        public async Task<IActionResult> Create(int appointmentId)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            var model = new MedicalRecordCreateViewModel
            {
                AppointmentId = appointmentId,
                PatientId = appointment.PatientId,
                PatientName = appointment.PatientName
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecordCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _medicalRecordService.CreateMedicalRecordAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }
            return View(medicalRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicalRecordViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _medicalRecordService.UpdateMedicalRecordAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }
            return View(medicalRecord);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _medicalRecordService.DeleteMedicalRecordAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PatientRecords(int patientId)
        {
            var records = await _medicalRecordService.GetMedicalRecordsByPatientAsync(patientId);
            return View(records);
        }
    }
}
