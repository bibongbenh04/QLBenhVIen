using HospitalManagement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Core.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentViewModel>> GetAllAppointmentsAsync();
        Task<IEnumerable<AppointmentViewModel>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<AppointmentViewModel>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<AppointmentViewModel>> GetAppointmentsByDateAsync(DateTime date);
        Task<AppointmentViewModel> GetAppointmentByIdAsync(int id);
        Task<int> CreateAppointmentAsync(AppointmentCreateViewModel model);
        Task UpdateAppointmentAsync(AppointmentViewModel model);
        Task DeleteAppointmentAsync(int id);
        Task<List<string>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
    }
}
