using HospitalManagement.Core.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HospitalManagement.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class MedicationsController : Controller
    {
        private readonly IMedicationService _medicationService;

        public MedicationsController(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        public async Task<IActionResult> Index()
        {
            var medications = await _medicationService.GetAllMedicationsAsync();
            return View(medications);
        }

        public async Task<IActionResult> Details(int id)
        {
            var medication = await _medicationService.GetMedicationByIdAsync(id);
            if (medication == null)
            {
                return NotFound();
            }
            return View(medication);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _medicationService.CreateMedicationAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var medication = await _medicationService.GetMedicationByIdAsync(id);
            if (medication == null)
            {
                return NotFound();
            }
            return View(medication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _medicationService.UpdateMedicationAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var medication = await _medicationService.GetMedicationByIdAsync(id);
            if (medication == null)
            {
                return NotFound();
            }
            return View(medication);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _medicationService.DeleteMedicationAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int id, int quantity)
        {
            await _medicationService.UpdateStockAsync(id, quantity);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var medications = await _medicationService.SearchMedicationsAsync(searchTerm);
            return View("Index", medications);
        }
    }
}
