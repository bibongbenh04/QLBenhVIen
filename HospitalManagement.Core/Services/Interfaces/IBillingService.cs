using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Core.Services.Interfaces
{
    public interface IBillingService
    {
        Task<IEnumerable<BillViewModel>> GetAllBillsAsync();
        Task<IEnumerable<BillViewModel>> GetBillsByPatientAsync(int patientId);
        Task<BillViewModel> GetBillByIdAsync(int id);
        Task<int> CreateBillAsync(BillCreateViewModel model);
        Task UpdateBillAsync(BillViewModel model);
        Task DeleteBillAsync(int id);
        Task<decimal> CalculateTotalRevenueAsync();
        Task<decimal> CalculateMonthlyRevenueAsync();
    }
}
