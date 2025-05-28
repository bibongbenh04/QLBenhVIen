using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
{
    [Authorize(Policy = "MedicalRecordsControllerAccess")]
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

        public async Task<IActionResult> Index(int? page, string? keyword)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var medicalRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                medicalRecords = medicalRecords.Where(u =>
                    !string.IsNullOrEmpty(u.PatientName) && u.PatientName.ToLower().Contains(keyword) || 
                    !string.IsNullOrEmpty(u.DoctorName) && u.DoctorName.ToLower().Contains(keyword) || 
                    !string.IsNullOrEmpty(u.AdmissionDate.ToString()) && u.AdmissionDate.ToString().ToLower().Contains(keyword)
                );
            }

            ViewBag.Keyword = keyword;

            var pagedList = medicalRecords.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
            
        }

        public async Task<IActionResult> Details(int id)
        {
            var medicalRecord = await _medicalRecordService.GetMedicalRecordDetailsAsync(id);
            return View(medicalRecord);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int appointmentId)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null) return NotFound();

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

                var appointment = await _appointmentService.GetAppointmentByIdAsync(model.AppointmentId);
                if (appointment != null)
                {
                    appointment.Status = "Đã hoàn thành";
                    await _appointmentService.UpdateAppointmentAsync(appointment);
                }

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

    }
}
