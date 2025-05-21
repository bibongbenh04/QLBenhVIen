using HospitalManagement.Services.Interfaces;
using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class TestService : ITestService
    {
        private readonly IRepository<Test> _testRepository;

        public TestService(IRepository<Test> testRepository)
        {
            _testRepository = testRepository;
        }

        public async Task<IEnumerable<TestViewModel>> GetTestByMedicalRecordIdAsync(int medicalRecordId)
        {
            var tests = await _testRepository.Query()
                            .Include(t => t.MedicalRecord)
                        .ThenInclude(m => m.Patient)
                    .Include(t => t.MedicalRecord)
                        .ThenInclude(m => m.Appointment)
                            .ThenInclude(a => a.Doctor)
                    .Where(t => t.MedicalRecordId == medicalRecordId)
                    .ToListAsync();

            return tests.Select(t => new TestViewModel
            {
                Id = t.Id,
                MedicalRecordId = t.MedicalRecordId,
                MedicalRecord = t.MedicalRecord,
                ServiceId = t.ServiceId,
                Service = t.Service,
                Price = t.Service?.Price ?? 0,
                Results = t.Results,
                Status = t.Status,
                TestDate = t.TestDate
            });
        }

        public async Task<int> CreateTestAsync(TestCreateViewModel model)
        {
            var entity = new Test
            {
                MedicalRecordId = model.MedicalRecordId,
                ServiceId = model.ServiceId,
                Status = "Ordered",
                TestDate = DateTime.Now,
                Results = ""
            };

            await _testRepository.AddAsync(entity);
            await _testRepository.SaveAsync();
            return entity.Id;
        }

        public async Task<TestViewModel> GetTestByIdAsync(int id)
        {
            var t = await _testRepository.Query().Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return null;

            return new TestViewModel
            {
                Id = t.Id,
                MedicalRecordId = t.MedicalRecordId,
                ServiceId = t.ServiceId,
                Service = t.Service,
                Price = t.Service?.Price ?? 0,
                Results = t.Results,
                Status = t.Status,
                TestDate = t.TestDate
            };
        }

        public async Task UpdateTestAsync(TestViewModel model)
        {
            var entity = await _testRepository.GetByIdAsync(model.Id);
            if (entity == null) return;

            entity.ServiceId = model.ServiceId;
            entity.Results = model.Results;
            entity.Status = model.Status;

            await _testRepository.UpdateAsync(entity);
            await _testRepository.SaveAsync();
        }

        public Task<IEnumerable<TestViewModel>> GetAllTestsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteTestAsync(int id)
        {
            var entity = await _testRepository.GetByIdAsync(id);
            if (entity == null) return;
            await _testRepository.DeleteAsync(entity);
            await _testRepository.SaveAsync();
        }


        public async Task SaveAsync()
        {
            await _testRepository.SaveAsync();
        }


        public async Task<List<Service>> GetAvailableServicesAsync()
        {
            return await _testRepository.Context().Set<Service>().ToListAsync();
        }

    }

}