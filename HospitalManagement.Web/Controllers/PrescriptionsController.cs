using HospitalManagement.Core.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HospitalManagement.Web.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class PrescriptionsController : Controller
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IMedicationService _medicationService;

        public PrescriptionsController(
            IPrescriptionService prescriptionService,
            IMedicationService medicationService)
        {
            _prescriptionService = prescriptionService;
            _medicationService = medicationService;
        }

        public async Task<IActionResult> Index()
        {
            var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync();
            return View(prescriptions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }
            return View(prescription);
        }

        public async Task<IActionResult> Create(int medicalRecordId, int doctorId)
        {
            var medications = await _medicationService.GetAllMedicationsAsync();
            var model = new PrescriptionCreateViewModel
            {
                MedicalRecordId = medicalRecordId,
                DoctorId = doctorId,
                Items = new List<PrescriptionItemCreateViewModel>(),
                AvailableMedications = medications.ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrescriptionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _prescriptionService.CreatePrescriptionAsync(model);
                return RedirectToAction(nameof(Index));
            }

            model.AvailableMedications = (await _medicationService.GetAllMedicationsAsync()).ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }
            return View(prescription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PrescriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _prescriptionService.UpdatePrescriptionAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }
            return View(prescription);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _prescriptionService.DeletePrescriptionAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PatientPrescriptions(int patientId)
        {
            var prescriptions = await _prescriptionService.GetPrescriptionsByPatientAsync(patientId);
            return View(prescriptions);
        }
    }
}
