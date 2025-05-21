using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

[Authorize(Roles = "Admin")]
public class ServicesController : Controller
{
    private readonly IServiceCatalogService _serviceCatalogService;

    public ServicesController(IServiceCatalogService serviceCatalogService)
    {
        _serviceCatalogService = serviceCatalogService;
    }

    public async Task<IActionResult> Index(int? page)
    {
        int pageNumber = page ?? 1;
        int pageSize = 5;

        var services = await _serviceCatalogService.GetAllAsync();
        var pagedList = services.ToPagedList(pageNumber, pageSize);
        return View(pagedList);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(ServiceViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await _serviceCatalogService.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id) => View(await _serviceCatalogService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Edit(ServiceViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await _serviceCatalogService.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id) => View(await _serviceCatalogService.GetByIdAsync(id));

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _serviceCatalogService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
