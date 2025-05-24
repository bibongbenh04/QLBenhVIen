using HospitalManagement.Services.Interfaces;
using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IRepository<MedicalRecord> _medicalRecordRepository;
        private readonly IRepository<Bill> _billRepository;

        public PatientService(
            IRepository<Patient> patientRepository,
            IRepository<Appointment> appointmentRepository,
            IRepository<MedicalRecord> medicalRecordRepository,
            IRepository<Bill> billRepository)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _billRepository = billRepository;
        }

        public async Task<IEnumerable<PatientViewModel>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAsync(p => p.IsActive);
            return patients.Select(MapToViewModel);
        }


        public async Task<PatientViewModel> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null) return null;
            return MapToViewModel(patient);
        }

        public async Task<IEnumerable<PatientViewModel>> GetPatientsByDoctorAsync(string doctorUserId)
        {
            var doctor = await _appointmentRepository.Query()
                .Include(a => a.Doctor)
                .Where(a => a.Doctor.UserId == doctorUserId && a.IsActive)
                .Select(a => a.Patient)
                .Distinct()
                .ToListAsync();

            return doctor.Select(MapToViewModel);
        }

        public async Task<PatientDetailsViewModel> GetPatientDetailsAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return new PatientDetailsViewModel();

            var appointments = await _appointmentRepository.GetAsync(
                a => a.PatientId == id,
                null!,
                "Doctor");

            var medicalRecords = await _medicalRecordRepository.GetAsync(
                m => m.PatientId == id,
                null!,
                "Appointment");

            var bills = await _billRepository.GetAsync(
                b => b.PatientId == id,
                null!,
                "BillItems");

            return new PatientDetailsViewModel
            {
                Patient = MapToViewModel(patient),
                Appointments = appointments.Select(a => new AppointmentViewModels
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    DoctorId = a.DoctorId,
                    DoctorName = $"{a.Doctor.FirstName} {a.Doctor.LastName}",
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Reason = a.Reason,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                }).ToList(),
                MedicalRecords = medicalRecords.Select(m => new MedicalRecordViewModel
                {
                    Id = m.Id,
                    PatientId = m.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    AppointmentId = m.AppointmentId,
                    Diagnosis = m.Diagnosis,
                    Treatment = m.Treatment,
                    Notes = m.Notes,
                    RecordDate = m.RecordDate
                }).ToList(),
                Bills = bills.Select(b => new BillViewModel
                {
                    Id = b.Id,
                    PatientId = b.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    BillDate = b.BillDate,
                    TotalAmount = b.TotalAmount,
                    PaidAmount = b.PaidAmount,
                    DueAmount = b.DueAmount,
                    InsuranceCoverage = b.InsuranceCoverage,
                    PaymentStatus = b.PaymentStatus,
                    PaymentMethod = b.PaymentMethod,
                    Items = b.BillItems.Select(bi => new BillItemViewModel
                    {
                        Id = bi.Id,
                        BillId = bi.BillId,
                        ItemName = bi.ItemName,
                        ItemType = bi.ItemType,
                        Quantity = bi.Quantity,
                        UnitPrice = bi.UnitPrice,
                        Subtotal = bi.Subtotal
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<Patient> CreatePatientAsync(PatientViewModel model, string userId)
        {
            
            var patient = new Patient
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Address = model.Address,
                EmergencyContactName = model.EmergencyContactName,
                EmergencyContactPhone = model.EmergencyContactPhone,
                BloodGroup = model.BloodGroup,
                HasInsurance = model.HasInsurance,
                InsuranceProvider = model.InsuranceProvider,
                InsurancePolicyNumber = model.InsurancePolicyNumber,
                UserId = userId
            };

            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveAsync();
            return patient;
        }

        public async Task UpdatePatientAsync(PatientViewModel model)
        {
            var patient = await _patientRepository.GetByIdAsync(model.Id);
            if (patient == null)
                throw new Exception("Patient not found");

            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.DateOfBirth = model.DateOfBirth;
            patient.Gender = model.Gender;
            patient.PhoneNumber = model.PhoneNumber;
            patient.Email = model.Email;
            patient.Address = model.Address;
            patient.EmergencyContactName = model.EmergencyContactName;
            patient.EmergencyContactPhone = model.EmergencyContactPhone;
            patient.BloodGroup = model.BloodGroup;
            patient.HasInsurance = model.HasInsurance;
            patient.InsuranceProvider = model.InsuranceProvider;
            patient.InsurancePolicyNumber = model.InsurancePolicyNumber;

            await _patientRepository.UpdateAsync(patient);
            await _patientRepository.SaveAsync();
        }

        public async Task DeletePatientAsync(int id)
        {
            // await _patientRepository.DeleteAsync(id);
            var p = await _patientRepository.GetByIdAsync(id);
            p.IsActive = false;
            await _patientRepository.SaveAsync();
        }

        private PatientViewModel MapToViewModel(Patient patient)
        {
            return new PatientViewModel
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                Address = patient.Address,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactPhone = patient.EmergencyContactPhone,
                BloodGroup = patient.BloodGroup,
                HasInsurance = patient.HasInsurance,
                InsuranceProvider = patient.InsuranceProvider,
                InsurancePolicyNumber = patient.InsurancePolicyNumber
            };
        }
    }
}