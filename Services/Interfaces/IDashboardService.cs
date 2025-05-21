using HospitalManagement.Models.ViewModels;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}
