using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Interfaces
{
    public interface IBillingService
    {
        Task<IEnumerable<BillViewModel>> GetAllBillsAsync();
        Task<IEnumerable<BillViewModel>> GetBillsByPatientIdAsync(int patientId);
        Task<BillViewModel> GetBillByIdAsync(int id);
        Task<int> CreateBillAsync(BillCreateViewModel model);
        Task<int> CreateBillAsync(BillCreateViewModel model, int medicalRecordId);
        Task UpdateBillAsync(BillViewModel model);
        Task DeleteBillAsync(int id);
        Task ApplyPaymentAsync(int billId, decimal newPaidAmount, string paymentMethod);
        Task<decimal> CalculateTotalRevenueAsync();
        Task<decimal> CalculateMonthlyRevenueAsync();
        Task<BillCreateViewModel> CreateBillFromMedicalRecordAsync(int medicalRecordId);
        Task<BillViewModel> GetBillByMedicalRecordIdAsync(int medicalRecordId);
        Task<int> AddBillWithEncryptionAsync(BillCreateViewModel model);
    }
}
