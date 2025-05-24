using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using HospitalManagement.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class ServiceCatalogService : IServiceCatalogService
    {
        private readonly IRepository<Service> _repo;

        public ServiceCatalogService(IRepository<Service> repo)
        {
            _repo = repo;
        }

        public async Task<List<ServiceViewModel>> GetAllAsync()
        {
            var list = await _repo.GetAsync(s => s.IsActive);
            return list.Select(s => new ServiceViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price
            }).ToList();
        }

        public async Task<ServiceViewModel> GetByIdAsync(int id)
        {
            var s = await _repo.GetByIdAsync(id);
            return new ServiceViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price
            };
        }

        public async Task CreateAsync(ServiceViewModel model)
        {
            var s = new Service { Name = model.Name, Description = model.Description, Price = model.Price };
            await _repo.AddAsync(s);
            await _repo.SaveAsync();
        }

        public async Task UpdateAsync(ServiceViewModel model)
        {
            var s = await _repo.GetByIdAsync(model.Id);
            s.Name = model.Name;
            s.Description = model.Description;
            s.Price = model.Price;
            await _repo.UpdateAsync(s);
            await _repo.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var s = await _repo.GetByIdAsync(id);
            // await _repo.DeleteAsync(s);
            s.IsActive = false;
            await _repo.SaveAsync();
        }
    }
}