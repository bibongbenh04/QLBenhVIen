using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<MedicalRecordViewModel>> GetAllMedicalRecordsAsync();
        Task<IEnumerable<MedicalRecordViewModel>> GetMedicalRecordsByPatientIdAsync(int patientId);
        Task<MedicalRecordViewModel> GetMedicalRecordByIdAsync(int id);
        Task<MedicalRecordDetailsViewModel> GetMedicalRecordDetailsAsync(int id);
        Task<int> CreateMedicalRecordAsync(MedicalRecordCreateViewModel model);
        Task UpdateMedicalRecordAsync(MedicalRecordViewModel model);
        Task DeleteMedicalRecordAsync(int id);
    }
}

