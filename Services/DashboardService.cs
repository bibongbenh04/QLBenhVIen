using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.ViewModels;

namespace HospitalManagement.Services
{
    public class DashboardService : IDashboardService
    {
        public Task<DashboardViewModel> GetDashboardDataAsync()
        {
            // Trả về mẫu dữ liệu giả để test
            return Task.FromResult(new DashboardViewModel
            {
                TotalPatients = 100,
                TotalDoctors = 20,
                TotalAppointments = 50,
                TodayAppointments = 5,
                TotalRevenue = 123456.78M,
                MonthlyRevenue = 1234.56M,
                RecentAppointments = new List<AppointmentViewModels>(),
                AppointmentsByDepartment = new Dictionary<string, int>(),
                PatientsByGender = new Dictionary<string, int>(),
                RevenueByMonth = new Dictionary<string, decimal>()
            });
        }
    }
}
