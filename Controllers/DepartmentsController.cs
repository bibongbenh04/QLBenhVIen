using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
{
    [Authorize(Policy = "DepartmentsControllerAccess")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var depts = await _departmentService.GetAllDepartmentsAsync();
            var pagedList = depts.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModels model)
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
                await _departmentService.CreateDepartmentAsync(model, null);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _departmentService.GetDepartmentByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentViewModels model)
        {
            if (ModelState.IsValid)
            {
                await _departmentService.UpdateDepartmentAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await _departmentService.GetDepartmentByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
