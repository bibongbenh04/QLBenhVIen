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

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var medicalRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
            var pagedList = medicalRecords.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        public async Task<IActionResult> ListBill(int patientId, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var bills = await _billingService.GetBillsByPatientIdAsync(patientId);
            var pagedList = bills.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
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
                return View(model);
            }
            await _billingService.CreateBillAsync(model);
            return RedirectToAction("Index");
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
            return RedirectToAction("ListBill", new { patientId = model.PatientId });
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
                return RedirectToAction(nameof(Index));
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

            return RedirectToAction(nameof(Index));
        }

        // public async Task<IActionResult> PatientBills(int patientId)
        // {
        //     var bills = await _billingService.GetBillsByPatientAsync(patientId);
        //     return View(bills);
        // }
    }
}
