using HospitalManagement.Services.Interfaces;
using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;

namespace HospitalManagement.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepo;
        private readonly IRepository<Doctor> _doctorRepo;
        private readonly IRepository<Patient> _patientRepo;

        public AppointmentService(IRepository<Appointment> appointmentRepo, IRepository<Doctor> doctorRepo, IRepository<Patient> patientRepo)
        {
            _appointmentRepo = appointmentRepo;
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
        }

        public async Task<IEnumerable<AppointmentViewModels>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepo.GetAsync(null, null, "Doctor,Patient,MedicalRecord");

            return appointments.Select(a => new AppointmentViewModels
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                PatientId = a.PatientId,
                PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                Reason = a.Reason,
                Notes = a.Notes,
                Status = a.Status,
                HasMedicalRecord = a.MedicalRecord != null 
            });

        }

        public async Task<AppointmentViewModels> GetAppointmentByIdAsync(int id)
        {
            var a = await _appointmentRepo.GetFirstOrDefaultAsync(
                x => x.Id == id,
                "Patient,Doctor,MedicalRecord"
            );

            if (a == null) return null;

            return new AppointmentViewModels
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                PatientId = a.PatientId,
                PatientName = a.Patient?.FirstName + " " + a.Patient?.LastName, // fix null
                DoctorName = a.Doctor?.FirstName + " " + a.Doctor?.LastName,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                Reason = a.Reason,
                Notes = a.Notes,
                Status = a.Status,
                HasMedicalRecord = a.MedicalRecord != null
            };
        }


        public async Task CreateAppointmentAsync(AppointmentCreateViewModel model)
        {
            var entity = new Appointment
            {
                DoctorId = model.DoctorId,
                PatientId = model.PatientId,
                AppointmentDate = model.AppointmentDate.Date,
                AppointmentTime = TimeSpan.Parse(model.AppointmentTime),
                Reason = model.Reason,
                Notes = model.Notes,
                Status = "Chờ khám",
                CreatedAt = DateTime.Now
            };
            await _appointmentRepo.AddAsync(entity);
            await _appointmentRepo.SaveAsync();
        }

        public async Task UpdateAppointmentAsync(AppointmentViewModels model)
        {
            var a = await _appointmentRepo.GetByIdAsync(model.Id);
            if (a == null) return;

            a.AppointmentDate = model.AppointmentDate;
            a.AppointmentTime = model.AppointmentTime;
            a.Reason = model.Reason;
            a.Notes = model.Notes;
            a.Status = model.Status;
            await _appointmentRepo.UpdateAsync(a);
            await _appointmentRepo.SaveAsync();
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            var a = await _appointmentRepo.GetByIdAsync(id);
            await _appointmentRepo.DeleteAsync(a);
            await _appointmentRepo.SaveAsync();
        }

        public Task<IEnumerable<AppointmentViewModels>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppointmentViewModels>> GetAppointmentsByPatientAsync(int patientId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppointmentViewModels>> GetAppointmentsByDateAsync(DateTime date)
        {
            throw new NotImplementedException();
        }
        public Task<List<string>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            throw new NotImplementedException();
        }

    }

}
