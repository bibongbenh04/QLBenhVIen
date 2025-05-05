using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Core.Services.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientViewModel>> GetAllPatientsAsync();
        Task<PatientViewModel> GetPatientByIdAsync(int id);
        Task<PatientDetailsViewModel> GetPatientDetailsAsync(int id);
        Task<Patient> CreatePatientAsync(PatientViewModel model, string userId);
        Task UpdatePatientAsync(PatientViewModel model);
        Task DeletePatientAsync(int id);
    }
}
