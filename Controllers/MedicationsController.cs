using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
{
    [Authorize(Policy = "MedicationsControllerAccess")]
    public class MedicationsController : Controller
    {
        private readonly IMedicationService _medicationService;

        public MedicationsController(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        public async Task<IActionResult> Index(int? page, string? keyword)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var medications = await _medicationService.GetAllMedicationsAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                medications = medications.Where(u =>
                    !string.IsNullOrEmpty(u.Name) && u.Name.ToLower().Contains(keyword) || 
                    !string.IsNullOrEmpty(u.Description) && u.Description.ToLower().Contains(keyword) || 
                    !string.IsNullOrEmpty(u.Brand) && u.Brand.ToLower().Contains(keyword)
                );
            }

            ViewBag.Keyword = keyword;

            var pagedList = medications.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
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
