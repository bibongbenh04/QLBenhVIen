using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
{
    [Authorize(Policy = "PrescriptionsControllerAccess")]
    public class PrescriptionsController : Controller
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IMedicationService _medicationService;
        private readonly IMedicalRecordService _medicalRecordService;

        private readonly ITestService _testService;

        public PrescriptionsController(
            IPrescriptionService prescriptionService,
            IMedicationService medicationService,
            IMedicalRecordService medicalRecordService,
            ITestService testService)
        {
            _prescriptionService = prescriptionService;
            _medicationService = medicationService;
            _medicalRecordService = medicalRecordService;
            _testService = testService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int medicalRecordId)
        {
            var tests = await _testService.GetTestByMedicalRecordIdAsync(medicalRecordId);
            if (tests.Any(t => t.Status != "Completed" && t.Status != "Cancelled"))
            {
                TempData["ErrorMessage"] = "Không thể tạo đơn thuốc. Một số xét nghiệm chưa hoàn thành.";
                return RedirectToAction("Index");
            }

            // Lấy hồ sơ để lấy DoctorId
            var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(medicalRecordId);
            if (medicalRecord == null)
            {
                return NotFound();
            }



            var medications = await _medicationService.GetAllMedicationsAsync();
            if (!medications.Any())
            {
                TempData["ErrorMessage"] = "Không thể tạo đơn thuốc vì chưa có danh sách thuốc. Vui lòng thêm thuốc trước.";
                return RedirectToAction("Index");
            }

            var model = new PrescriptionCreateViewModel
            {
                MedicalRecordId = medicalRecordId,
                DoctorId = medicalRecord.DoctorId,
                Items = new List<PrescriptionItemCreateViewModel>(),
                AvailableMedications = medications.ToList()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrescriptionCreateViewModel model)
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
            if (ModelState.IsValid)
            {
                await _prescriptionService.CreatePrescriptionAsync(model);
                return RedirectToAction(nameof(Index));
            }

            model.AvailableMedications = (await _medicationService.GetAllMedicationsAsync()).ToList();
            return View(model);
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

        public async Task<IActionResult> ListPrescription(int medicalRecordId, int? page, string? keyword)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var prescriptions = await _prescriptionService.GetAllPrescriptionsByMedicalRecordIdAsync(medicalRecordId);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                prescriptions = prescriptions.Where(u =>
                    !string.IsNullOrEmpty(u.DoctorName) && u.DoctorName.ToLower().Contains(keyword) || 
                    !string.IsNullOrEmpty(u.PrescriptionDate.ToString()) && u.PrescriptionDate.ToString().ToLower().Contains(keyword)
                );
            }

            ViewBag.Keyword = keyword;

            var pagedList = prescriptions.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet("Prescriptions/DetailsByRecord")]
        public async Task<IActionResult> Details(int patientId, int doctorId, DateTime recordDate)
        {
            var prescription = await _prescriptionService.GetPrescriptionByPatientAndDoctorAndDateAsync(patientId, doctorId, recordDate);
            if (prescription == null)
            {
                TempData["ErrorMessage"] = "Không thể xem chi tiết đơn thuốc vì chưa có danh sách thuốc. Vui lòng thêm thuốc vào danh sách.";
                return RedirectToAction("Index", new { noMedication = "true" });
                // return NotFound();
            }
            return View("Details", prescription);
        }
        
        [HttpGet("Prescriptions/Details")]
        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }
            return View(prescription);
        }

        // public async Task<IActionResult> Create(int medicalRecordId, int doctorId)
        // {
        //     var medications = await _medicationService.GetAllMedicationsAsync();
        //     var model = new PrescriptionCreateViewModel
        //     {
        //         MedicalRecordId = medicalRecordId,
        //         DoctorId = doctorId,
        //         Items = new List<PrescriptionItemCreateViewModel>(),
        //         AvailableMedications = medications.ToList()
        //     };
        //     return View(model);
        // }



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

/*
// Controllers/PrescriptionsController.cs
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class PrescriptionsController : Controller
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IMedicationService _medicationService;
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly ITestService _testService;

        public PrescriptionsController(
            IPrescriptionService prescriptionService,
            IMedicationService medicationService,
            IMedicalRecordService medicalRecordService,
            ITestService testService)
        {
            _prescriptionService = prescriptionService;
            _medicationService = medicationService;
            _medicalRecordService = medicalRecordService;
            _testService = testService;
        }

        public async Task<IActionResult> Create(int medicalRecordId)
        {
            var tests = await _testService.GetTestByMedicalRecordIdAsync(medicalRecordId);
            if (tests.Any(t => t.Status != "Completed" && t.Status != "Cancelled"))
            {
                TempData["ErrorMessage"] = "Không thể tạo đơn thuốc. Một số xét nghiệm chưa hoàn thành.";
                return RedirectToAction("Index");
            }

            var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(medicalRecordId);
            if (medicalRecord == null) return NotFound();

            var medications = await _medicationService.GetAllMedicationsAsync();

            var model = new PrescriptionCreateViewModel
            {
                MedicalRecordId = medicalRecordId,
                DoctorId = medicalRecord.DoctorId,
                Items = new List<PrescriptionItemCreateViewModel>(),
                AvailableMedications = medications.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrescriptionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableMedications = (await _medicationService.GetAllMedicationsAsync()).ToList();
                return View(model);
            }

            await _prescriptionService.CreatePrescriptionAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;
            var medicalRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
            return View(medicalRecords.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> ListPrescription(int medicalRecordId, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;
            var prescriptions = await _prescriptionService.GetAllPrescriptionsByMedicalRecordIdAsync(medicalRecordId);
            return View(prescriptions.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Details(int patientId, int doctorId, DateTime visitDate)
        {
            var prescription = await _prescriptionService.GetPrescriptionByPatientAndDoctorAndDateAsync(patientId, doctorId, visitDate);
            if (prescription == null) return NotFound();
            return View(prescription);
        }

        public async Task<IActionResult> Edit(int patientId, int doctorId, DateTime visitDate)
        {
            var prescription = await _prescriptionService.GetPrescriptionByPatientAndDoctorAndDateAsync(patientId, doctorId, visitDate);
            if (prescription == null) return NotFound();
            return View(prescription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PrescriptionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _prescriptionService.UpdatePrescriptionAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int patientId, int doctorId, DateTime visitDate)
        {
            var prescription = await _prescriptionService.GetPrescriptionByPatientAndDoctorAndDateAsync(patientId, doctorId, visitDate);
            if (prescription == null) return NotFound();
            return View(prescription);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int patientId, int doctorId, DateTime visitDate)
        {
            var prescription = await _prescriptionService.GetPrescriptionByPatientAndDoctorAndDateAsync(patientId, doctorId, visitDate);
            if (prescription == null) return NotFound();
            await _prescriptionService.DeletePrescriptionAsync(prescription.Id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PatientPrescriptions(int patientId)
        {
            var prescriptions = await _prescriptionService.GetPrescriptionsByPatientAsync(patientId);
            return View(prescriptions);
        }
    }
}

*/