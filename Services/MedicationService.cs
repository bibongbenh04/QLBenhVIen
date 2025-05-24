using HospitalManagement.Services.Interfaces;
using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly IRepository<Medication> _medicationRepo;

        public MedicationService(IRepository<Medication> medicationRepo)
        {
            _medicationRepo = medicationRepo;
        }

        public async Task<int> CreateMedicationAsync(MedicationViewModel model)
        {
            var entity = new Medication
            {
                Name = model.Name,
                GenericName = model.GenericName,
                Brand = model.Brand,
                Category = model.Category,
                Description = model.Description,
                DosageForm = model.DosageForm,
                Strength = model.Strength,
                UnitPrice = model.UnitPrice,
                StockQuantity = model.StockQuantity,
                ReorderLevel = model.ReorderLevel
            };

            await _medicationRepo.AddAsync(entity);
            await _medicationRepo.SaveAsync();
            return entity.Id;
        }

        public async Task UpdateMedicationAsync(MedicationViewModel model)
        {
            var entity = await _medicationRepo.GetByIdAsync(model.Id);
            if (entity == null) return;

            entity.Name = model.Name;
            entity.GenericName = model.GenericName;
            entity.Brand = model.Brand;
            entity.Category = model.Category;
            entity.Description = model.Description;
            entity.DosageForm = model.DosageForm;
            entity.Strength = model.Strength;
            entity.UnitPrice = model.UnitPrice;
            entity.StockQuantity = model.StockQuantity;
            entity.ReorderLevel = model.ReorderLevel;

            await _medicationRepo.UpdateAsync(entity);
            await _medicationRepo.SaveAsync();
        }

        public async Task DeleteMedicationAsync(int id)
        {
            var entity = await _medicationRepo.GetByIdAsync(id);
            if (entity == null) return;

            // await _medicationRepo.DeleteAsync(entity);
            entity.IsActive = false;
            await _medicationRepo.SaveAsync();
        }

        public async Task<IEnumerable<MedicationViewModel>> GetAllMedicationsAsync()
        {
            var data = await _medicationRepo.GetAsync(m => m.IsActive);
            return data.Select(m => new MedicationViewModel
            {
                Id = m.Id,
                Name = m.Name,
                GenericName = m.GenericName,
                Brand = m.Brand,
                Category = m.Category,
                Description = m.Description,
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                UnitPrice = m.UnitPrice,
                StockQuantity = m.StockQuantity,
                ReorderLevel = m.ReorderLevel
            });
        }

        public async Task<MedicationViewModel> GetMedicationByIdAsync(int id)
        {
            var m = await _medicationRepo.GetByIdAsync(id);
            if (m == null) return null;

            return new MedicationViewModel
            {
                Id = m.Id,
                Name = m.Name,
                GenericName = m.GenericName,
                Brand = m.Brand,
                Category = m.Category,
                Description = m.Description,
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                UnitPrice = m.UnitPrice,
                StockQuantity = m.StockQuantity,
                ReorderLevel = m.ReorderLevel
            };
        }

        public async Task<IEnumerable<MedicationViewModel>> SearchMedicationsAsync(string searchTerm)
        {
            var data = await _medicationRepo.GetAsync(
                m => (m.Name.Contains(searchTerm) || m.GenericName.Contains(searchTerm)) && m.IsActive);

            return data.Select(m => new MedicationViewModel
            {
                Id = m.Id,
                Name = m.Name,
                GenericName = m.GenericName,
                Brand = m.Brand,
                Category = m.Category,
                Description = m.Description,
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                UnitPrice = m.UnitPrice,
                StockQuantity = m.StockQuantity,
                ReorderLevel = m.ReorderLevel
            });
        }

        public async Task UpdateStockAsync(int id, int quantity)
        {
            var med = await _medicationRepo.GetByIdAsync(id);
            if (med == null) return;

            med.StockQuantity = quantity;
            await _medicationRepo.UpdateAsync(med);
            await _medicationRepo.SaveAsync();
        }
    }
}
