using HospitalManagement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentViewModels>> GetAllAppointmentsAsync();
        Task<IEnumerable<AppointmentViewModels>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<AppointmentViewModels>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<AppointmentViewModels>> GetAppointmentsByDateAsync(DateTime date);

        Task<AppointmentViewModels> GetAppointmentByIdAsync(int id);
        Task CreateAppointmentAsync(AppointmentCreateViewModel model);
        Task UpdateAppointmentAsync(AppointmentViewModels model);
        Task DeleteAppointmentAsync(int id);
        Task<List<string>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
        Task<bool> HasExistingAppointment(int patientId, DateTime date);
    }
}
