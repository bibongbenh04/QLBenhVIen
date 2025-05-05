using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Core.Services.Interfaces
{
    public interface IMedicationService
    {
        Task<IEnumerable<MedicationViewModel>> GetAllMedicationsAsync();
        Task<MedicationViewModel> GetMedicationByIdAsync(int id);
        Task<int> CreateMedicationAsync(MedicationViewModel model);
        Task UpdateMedicationAsync(MedicationViewModel model);
        Task DeleteMedicationAsync(int id);
        Task<IEnumerable<MedicationViewModel>> SearchMedicationsAsync(string searchTerm);
        Task UpdateStockAsync(int id, int quantity);
    }
}
