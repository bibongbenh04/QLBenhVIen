using System.Collections.Generic;

namespace HospitalManagement.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<AppointmentViewModels> RecentAppointments { get; set; }
        public Dictionary<string, int> AppointmentsByDepartment { get; set; }
        public Dictionary<string, int> PatientsByGender { get; set; }
        public Dictionary<string, decimal> RevenueByMonth { get; set; }
    }
}
