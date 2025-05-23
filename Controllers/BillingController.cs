using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class BillingController : Controller
    {
        private readonly IBillingService _billingService;
        private readonly IPatientService _patientService;
        private readonly IMedicalRecordService _medicalRecordService;

        public BillingController(
            IBillingService billingService,
            IPatientService patientService,
            IMedicalRecordService medicalRecordService)
        {
            _billingService = billingService;
            _patientService = patientService;
            _medicalRecordService = medicalRecordService;
        }

        public async Task<IActionResult> ListPatient(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var patients = await _patientService.GetAllPatientsAsync();
            var pagedList = patients.ToPagedList(pageNumber, pageSize);
            return View(pagedList);

        }

        public async Task<IActionResult> ListMedicalRecord(int patientId, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var medicalRecords = await _medicalRecordService.GetMedicalRecordsByPatientIdAsync(patientId);
            var pagedList = medicalRecords.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        public async Task<IActionResult> Bill(int medicalRecordId)
        {
            var bill = await _billingService.GetBillByMedicalRecordIdAsync(medicalRecordId);
            if (bill == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hóa đơn cho hồ sơ này.";
                var record = await _medicalRecordService.GetMedicalRecordByIdAsync(medicalRecordId);
                if (record != null)
                {
                    return RedirectToAction("ListMedicalRecord", new { patientId = record.PatientId });
                }
                return RedirectToAction("ListPatient");
            }

            return View(bill);
        }



        public async Task<IActionResult> Details(int id)
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        [HttpGet]
        public async Task<IActionResult> CreateFromMedicalRecord(int medicalRecordId)
        {
            var bill = await _billingService.CreateBillFromMedicalRecordAsync(medicalRecordId);
            if (bill == null)
            {
                return NotFound();
            }
            return View("CreateFromMedicalRecord", bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateFromMedicalRecord(BillCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _billingService.CreateBillAsync(model, model.MedicalRecordId);

            return RedirectToAction("ListMedicalRecord", new { patientId = model.PatientId });
        }


        [HttpGet]
        public async Task<IActionResult> Pay(int id)
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null) return NotFound();
            return View("Pay", bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(BillViewModel model)
        {
            if (model.PaidAmount <= 0)
            {
                ModelState.AddModelError("PaidAmount", "Số tiền thanh toán phải lớn hơn 0.");
                return View(model);
            }

            var current = await _billingService.GetBillByIdAsync(model.Id);
            if (current == null) return NotFound();

            if (current.PaymentStatus == "Paid")
            {
                TempData["Error"] = "Hóa đơn đã được thanh toán đầy đủ.";
                return RedirectToAction("Pay", new { id = model.Id });
            }

            if (model.PaidAmount > current.DueAmount)
            {
                TempData["Error"] = $"Số tiền vượt quá số còn lại ({current.DueAmount:C})";
                ModelState.AddModelError("PaidAmount", $"Số tiền vượt quá số còn lại ({current.DueAmount:C})");
                return View(model);
            }

            await _billingService.ApplyPaymentAsync(model.Id, model.PaidAmount, model.PaymentMethod);

            TempData["Success"] = "Thanh toán thành công.";
            return RedirectToAction("Bill", new { medicalRecordId = model.MedicalRecordId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BillViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _billingService.UpdateBillAsync(model);
                return RedirectToAction(nameof(Bill), new { id = model.Id });
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _billingService.DeleteBillAsync(id);
                TempData["Success"] = "Đã xóa hóa đơn thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Không thể xóa hóa đơn: {ex.Message}";
            }

            return RedirectToAction(nameof(ListPatient));
        }

    }
}
