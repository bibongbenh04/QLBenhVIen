using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Interfaces
{
    public interface ITestService
    {
        Task<IEnumerable<TestViewModel>> GetAllTestsAsync();
        Task<TestViewModel> GetTestByIdAsync(int id);
        Task<IEnumerable<TestViewModel>> GetTestByMedicalRecordIdAsync(int medicalRecordId);
        Task<List<Service>> GetAvailableServicesAsync();

        Task<int> CreateTestAsync(TestCreateViewModel model);
        Task UpdateTestAsync(TestViewModel model);
        Task DeleteTestAsync(int id);
        Task SaveAsync();
    }

}
