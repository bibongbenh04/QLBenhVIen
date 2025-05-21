using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalManagement.Models.ViewModels;

public interface IServiceCatalogService
{
    Task<List<ServiceViewModel>> GetAllAsync();
    Task<ServiceViewModel> GetByIdAsync(int id);
    Task CreateAsync(ServiceViewModel model);
    Task UpdateAsync(ServiceViewModel model);
    Task DeleteAsync(int id);
}