using HospitalManagement.Services.Interfaces;
using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IRepository<MedicalRecord> _recordRepo;
        private readonly IRepository<Appointment> _appointmentRepo;
        private readonly IRepository<Patient> _patientRepo;

        public MedicalRecordService(
            IRepository<MedicalRecord> recordRepo,
            IRepository<Appointment> appointmentRepo,
            IRepository<Patient> patientRepo)
        {
            _recordRepo = recordRepo;
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
        }

        public async Task<IEnumerable<MedicalRecordViewModel>> GetAllMedicalRecordsAsync()
        {
            var records = await _recordRepo.GetAsync(null, null, "Patient,Appointment.Doctor");

            return records.Select(r => new MedicalRecordViewModel
            {
                Id = r.Id,
                PatientId = r.PatientId,
                PatientName = r.Patient != null ? r.Patient.FirstName + " " + r.Patient.LastName : "N/A",
                AppointmentId = r.AppointmentId,
                Diagnosis = r.Diagnosis,
                Treatment = r.Treatment,
                Notes = r.Notes,
                RecordDate = r.RecordDate,
                DoctorId = r.Appointment.DoctorId,
                DoctorName = r.Appointment?.Doctor != null
                    ? r.Appointment.Doctor.FirstName + " " + r.Appointment.Doctor.LastName
                    : "N/A",
                AdmissionDate = r.Appointment?.AppointmentDate ?? DateTime.MinValue,
                HasBill = r.Bill != null
            });
        }

        public async Task<IEnumerable<MedicalRecordViewModel>> GetMedicalRecordsByPatientIdAsync(int patientId)
        {
            var records = await _recordRepo.GetAsync(r => r.PatientId == patientId, null, "Appointment.Doctor,Patient,Bill");
            return records.Select(r => new MedicalRecordViewModel
            {
                Id = r.Id,
                PatientId = r.PatientId,
                PatientName = r.Patient.FirstName + " " + r.Patient.LastName,
                AppointmentId = r.AppointmentId,
                Diagnosis = r.Diagnosis,
                Treatment = r.Treatment,
                Notes = r.Notes,
                RecordDate = r.RecordDate,
                DoctorId = r.Appointment.DoctorId,
                DoctorName = r.Appointment.Doctor?.FirstName + " " + r.Appointment.Doctor?.LastName,
                AdmissionDate = r.Appointment.AppointmentDate,
                HasBill = r.Bill != null
            });
        }

        public async Task<MedicalRecordViewModel> GetMedicalRecordByIdAsync(int id)
        {
            var r = await _recordRepo.GetByIdAsync(id);
            if (r == null) return null;

            return new MedicalRecordViewModel
            {
                Id = r.Id,
                PatientId = r.PatientId,
                PatientName = r.Patient.FirstName + " " + r.Patient.LastName,
                AppointmentId = r.AppointmentId,
                Diagnosis = r.Diagnosis,
                Treatment = r.Treatment,
                Notes = r.Notes,
                RecordDate = r.RecordDate,
                DoctorId = r.Appointment.DoctorId,
                DoctorName = r.Appointment.Doctor?.FirstName + " " + r.Appointment.Doctor?.LastName,
                AdmissionDate = r.Appointment.AppointmentDate,
                HasBill = r.Bill != null
            };
        }

        public async Task<MedicalRecordDetailsViewModel> GetMedicalRecordDetailsAsync(int id)
        {
            var record = await _recordRepo.GetAsync(r => r.Id == id, null, "Patient,Appointment.Doctor");
            var r = record.FirstOrDefault();
            if (r == null) return null;

            var prescriptions = await _recordRepo.Context().Set<Prescription>()
                .Where(p => p.MedicalRecordId == id)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionItems)
                    .ThenInclude(i => i.Medication)
                .ToListAsync();

            var tests = await _recordRepo.Context().Set<Test>()
                .Include(t => t.Service)
                .Where(t => t.MedicalRecordId == id)
                .ToListAsync();

            return new MedicalRecordDetailsViewModel
            {
                MedicalRecord = new MedicalRecordViewModel
                {
                    Id = r.Id,
                    PatientId = r.PatientId,
                    PatientName = r.Patient.FirstName + " " + r.Patient.LastName,
                    AppointmentId = r.AppointmentId,
                    Diagnosis = r.Diagnosis,
                    Treatment = r.Treatment,
                    Notes = r.Notes,
                    RecordDate = r.RecordDate,
                    DoctorName = r.Appointment?.Doctor != null
                        ? r.Appointment.Doctor.FirstName + " " + r.Appointment.Doctor.LastName
                        : "N/A",
                    AdmissionDate = r.Appointment?.AppointmentDate ?? DateTime.MinValue,
                    HasBill = r.Bill != null
                },

                Prescriptions = prescriptions.Select(p => new PrescriptionViewModel
                {
                    Id = p.Id,
                    MedicalRecordId = p.MedicalRecordId,
                    DoctorId = p.DoctorId,
                    DoctorName = p.Doctor != null ? p.Doctor.FirstName + " " + p.Doctor.LastName : "N/A",
                    PrescriptionDate = p.PrescriptionDate,
                    Notes = p.Notes,
                    Items = p.PrescriptionItems.Select(i => new PrescriptionItemViewModel
                    {
                        Id = i.Id,
                        PrescriptionId = i.PrescriptionId,
                        MedicationId = i.MedicationId,
                        MedicationName = i.Medication.Name,
                        Dosage = i.Dosage,
                        Instructions = i.Instructions,
                        Quantity = i.Quantity,
                        DurationDays = i.DurationDays
                    }).ToList()
                }).ToList(),

                Tests = tests.Select(t => new TestViewModel
                {
                    Id = t.Id,
                    MedicalRecordId = t.MedicalRecordId,
                    ServiceId = t.ServiceId,
                    Service = t.Service,
                    Price = t.Service?.Price ?? 0,
                    TestDate = t.TestDate,
                    Results = t.Results,
                    Status = t.Status
                }).ToList()
            };
        }


        public async Task<int> CreateMedicalRecordAsync(MedicalRecordCreateViewModel model)
        {
            var entity = new MedicalRecord
            {
                AppointmentId = model.AppointmentId,
                PatientId = model.PatientId,
                Diagnosis = model.Diagnosis,
                Treatment = model.Treatment,
                Notes = model.Notes,
                RecordDate = DateTime.Now
            };
            await _recordRepo.AddAsync(entity);
            await _recordRepo.SaveAsync();

            var appointment = await _appointmentRepo.GetByIdAsync(model.AppointmentId);
            if (appointment != null)
            {
                appointment.Status = "Completed";
                await _appointmentRepo.UpdateAsync(appointment);
                await _appointmentRepo.SaveAsync();
            }

            return entity.Id;
        }

        public async Task UpdateMedicalRecordAsync(MedicalRecordViewModel model)
        {
            var entity = await _recordRepo.GetByIdAsync(model.Id);
            if (entity == null) return;

            entity.Diagnosis = model.Diagnosis;
            entity.Treatment = model.Treatment;
            entity.Notes = model.Notes;

            await _recordRepo.UpdateAsync(entity);
            await _recordRepo.SaveAsync();
        }

        public async Task DeleteMedicalRecordAsync(int id)
        {
            var entity = await _recordRepo.GetByIdAsync(id);
            if (entity == null) return;
            await _recordRepo.DeleteAsync(entity);
            await _recordRepo.SaveAsync();
        }
    }
}
