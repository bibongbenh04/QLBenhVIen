using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Interfaces
{
    public interface IDoctorPayrollService
    {
        Task<IEnumerable<DoctorPayroll>> GetAllAsync();
        Task<DoctorPayroll> GetByIdAsync(int id);
        Task<IEnumerable<DoctorPayrollViewModel>> GetAllByDoctorIdAsync(int doctorId);
        Task CreateAsync(DoctorPayroll payroll);
        Task UpdateAsync(DoctorPayrollViewModel payroll);
        Task DeleteAsync(int id);
        Task CreatePayrollWithAutoAppointmentsAsync(DoctorPayroll payroll);
        Task<IEnumerable<Appointment>> GetAsync(Expression<Func<Appointment, bool>> filter);
    }
}