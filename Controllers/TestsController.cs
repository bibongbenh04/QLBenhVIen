using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HospitalManagement.Models.Entities;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
{
    [Authorize(Policy = "TestsControllerAccess")]
    public class TestsController : Controller
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IMedicationService _medicationService;
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly ITestService _testService;

        public TestsController(
            IPrescriptionService prescriptionService,
            IMedicationService medicationService,
            IMedicalRecordService medicalRecordService,
            ITestService testService)
        {
            _prescriptionService = prescriptionService;
            _medicationService = medicationService;
            _testService = testService;
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

        public async Task<IActionResult> ListTest(int medicalRecordId, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var tests = await _testService.GetTestByMedicalRecordIdAsync(medicalRecordId);
            var pagedList = tests.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int medicalRecordId)
        {
            var services = await _testService.GetAvailableServicesAsync();
            ViewBag.Services = services;

            return View(new TestCreateViewModel { MedicalRecordId = medicalRecordId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TestCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _testService.CreateTestAsync(model);
            await _testService.SaveAsync();

            return RedirectToAction("Details", "MedicalRecords", new { id = model.MedicalRecordId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _testService.GetTestByIdAsync(id);
            model.Service = (await _testService.GetAvailableServicesAsync())
                            .FirstOrDefault(s => s.Id == model.ServiceId);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Services = await _testService.GetAvailableServicesAsync();
                return View(model);
            }

            await _testService.UpdateTestAsync(model);
            return RedirectToAction("Details", "MedicalRecords", new { id = model.MedicalRecordId });
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int medicalRecordId)
        {
            await _testService.DeleteTestAsync(id);
            return RedirectToAction("Details", "MedicalRecords", new { id = medicalRecordId });
        }



    }
    
}
