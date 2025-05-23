using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using HospitalManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IRepository<Prescription> _prescriptionRepository;
        private readonly IRepository<PrescriptionItem> _prescriptionItemRepository;
        private readonly IRepository<Medication> _medicationRepository;

        public PrescriptionService(
            IRepository<Prescription> prescriptionRepository,
            IRepository<PrescriptionItem> prescriptionItemRepository,
            IRepository<Medication> medicationRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _prescriptionItemRepository = prescriptionItemRepository;
            _medicationRepository = medicationRepository;
        }

        public async Task<int> CreatePrescriptionAsync(PrescriptionCreateViewModel model)
        {
            var prescription = new Prescription
            {
                MedicalRecordId = model.MedicalRecordId,
                DoctorId = model.DoctorId,
                Notes = model.Notes,
                PrescriptionDate = DateTime.Now,
                PrescriptionItems = new List<PrescriptionItem>()
            };

            foreach (var item in model.Items)
            {
                prescription.PrescriptionItems.Add(new PrescriptionItem
                {
                    MedicationId = item.MedicationId,
                    Dosage = item.Dosage,
                    Instructions = item.Instructions,
                    Quantity = item.Quantity,
                    DurationDays = item.DurationDays
                });

                var med = await _medicationRepository.GetByIdAsync(item.MedicationId);
                if (med != null)
                {
                    med.StockQuantity -= item.Quantity;
                    await _medicationRepository.UpdateAsync(med);
                }
            }

            await _prescriptionRepository.AddAsync(prescription);
            await _prescriptionRepository.SaveAsync();

            return prescription.Id;
        }

        public async Task<IEnumerable<PrescriptionViewModel>> GetAllPrescriptionsAsync()
        {
            var list = await _prescriptionRepository.GetAsync(null, null, "Doctor,PrescriptionItems.Medication");

            return list.Select(p => new PrescriptionViewModel
            {
                Id = p.Id,
                MedicalRecordId = p.MedicalRecordId,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor?.FirstName + " " + p.Doctor?.LastName,
                Notes = p.Notes,
                PrescriptionDate = p.PrescriptionDate,
                Items = p.PrescriptionItems.Select(i => new PrescriptionItemViewModel
                {
                    Id = i.Id,
                    MedicationId = i.MedicationId,
                    MedicationName = i.Medication?.Name,
                    Dosage = i.Dosage,
                    Instructions = i.Instructions,
                    Quantity = i.Quantity,
                    DurationDays = i.DurationDays
                }).ToList()
            });
        }

        public async Task<PrescriptionViewModel> GetPrescriptionByIdAsync(int id)
        {
            var p = (await _prescriptionRepository.GetAsync(x => x.Id == id, null, "Doctor,PrescriptionItems.Medication"))
                .FirstOrDefault();

            if (p == null) return null;

            return new PrescriptionViewModel
            {
                Id = p.Id,
                MedicalRecordId = p.MedicalRecordId,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor?.FirstName + " " + p.Doctor?.LastName,
                Notes = p.Notes,
                PrescriptionDate = p.PrescriptionDate,
                Items = p.PrescriptionItems.Select(i => new PrescriptionItemViewModel
                {
                    Id = i.Id,
                    MedicationId = i.MedicationId,
                    MedicationName = i.Medication?.Name,
                    Dosage = i.Dosage,
                    Instructions = i.Instructions,
                    Quantity = i.Quantity,
                    DurationDays = i.DurationDays
                }).ToList()
            };
        }

        public async Task<PrescriptionViewModel> GetPrescriptionByPatientAndDoctorAndDateAsync(int patientId, int doctorId, DateTime recordDate)
        {
            var prescription = await _prescriptionRepository.Query()
                .Include(p => p.Doctor)
                .Include(p => p.MedicalRecord)
                .Include(p => p.PrescriptionItems)
                    .ThenInclude(i => i.Medication)
                .Where(p =>
                    p.DoctorId == doctorId &&
                    p.MedicalRecord.PatientId == patientId &&
                    p.MedicalRecord.RecordDate.Date == recordDate.Date)
                .FirstOrDefaultAsync();

            if (prescription == null)
                return null;

            return new PrescriptionViewModel
            {
                Id = prescription.Id,
                MedicalRecordId = prescription.MedicalRecordId,
                DoctorId = prescription.DoctorId,
                DoctorName = prescription.Doctor?.FirstName + " " + prescription.Doctor?.LastName,
                Notes = prescription.Notes,
                PrescriptionDate = prescription.PrescriptionDate,
                Items = prescription.PrescriptionItems.Select(i => new PrescriptionItemViewModel
                {
                    Id = i.Id,
                    MedicationId = i.MedicationId,
                    MedicationName = i.Medication?.Name,
                    Dosage = i.Dosage,
                    Instructions = i.Instructions,
                    Quantity = i.Quantity,
                    DurationDays = i.DurationDays
                }).ToList()
            };
        }

        public async Task<IEnumerable<PrescriptionViewModel>> GetAllPrescriptionsByMedicalRecordIdAsync(int medicalRecordId)
        {
            var list = await _prescriptionRepository.GetAsync(x => x.MedicalRecordId == medicalRecordId, null, "Doctor,PrescriptionItems.Medication");

            return list.Select(p => new PrescriptionViewModel
            {
                Id = p.Id,
                MedicalRecordId = p.MedicalRecordId,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor?.FirstName + " " + p.Doctor?.LastName,
                Notes = p.Notes,
                PrescriptionDate = p.PrescriptionDate,
                Items = p.PrescriptionItems.Select(i => new PrescriptionItemViewModel
                {
                    Id = i.Id,
                    MedicationId = i.MedicationId,
                    MedicationName = i.Medication?.Name,
                    Dosage = i.Dosage,
                    Instructions = i.Instructions,
                    Quantity = i.Quantity,
                    DurationDays = i.DurationDays
                }).ToList()
            });
        }


        public Task DeletePrescriptionAsync(int id)
        {
            // return _prescriptionRepository.DeleteByIdAsync(id);
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PrescriptionViewModel>> GetPrescriptionsByDoctorAsync(int doctorId)
        {
            return GetAllPrescriptionsAsync(); // Simplified for now
        }

        public Task<IEnumerable<PrescriptionViewModel>> GetPrescriptionsByPatientAsync(int patientId)
        {
            return GetAllPrescriptionsAsync(); // Simplified for now
        }

        public Task UpdatePrescriptionAsync(PrescriptionViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
