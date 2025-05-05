using HospitalManagement.Core.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HospitalManagement.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class BillingController : Controller
    {
        private readonly IBillingService _billingService;
        private readonly IPatientService _patientService;

        public BillingController(
            IBillingService billingService,
            IPatientService patientService)
        {
            _billingService = billingService;
            _patientService = patientService;
        }

        public async Task<IActionResult> Index()
        {
            var bills = await _billingService.GetAllBillsAsync();
            return View(bills);
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

        public async Task<IActionResult> Create()
        {
            var model = new BillCreateViewModel
            {
                Patients = (await _patientService.GetAllPatientsAsync()).ToList(),
                Items = new List<BillItemCreateViewModel>()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BillCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _billingService.CreateBillAsync(model);
                return RedirectToAction(nameof(Index));
            }

            model.Patients = (await _patientService.GetAllPatientsAsync()).ToList();
            return View(model);
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
            await _billingService.DeleteBillAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PatientBills(int patientId)
        {
            var bills = await _billingService.GetBillsByPatientAsync(patientId);
            return View(bills);
        }
    }
}
