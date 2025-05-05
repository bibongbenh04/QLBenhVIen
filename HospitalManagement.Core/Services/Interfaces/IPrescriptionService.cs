using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Core.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<PrescriptionViewModel>> GetAllPrescriptionsAsync();
        Task<IEnumerable<PrescriptionViewModel>> GetPrescriptionsByPatientAsync(int patientId);
        Task<IEnumerable<PrescriptionViewModel>> GetPrescriptionsByDoctorAsync(int doctorId);
        Task<PrescriptionViewModel> GetPrescriptionByIdAsync(int id);
        Task<int> CreatePrescriptionAsync(PrescriptionCreateViewModel model);
        Task UpdatePrescriptionAsync(PrescriptionViewModel model);
        Task DeletePrescriptionAsync(int id);
    }
}
