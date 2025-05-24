using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using HospitalManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class DoctorPayrollService : IDoctorPayrollService
    {
        private readonly IRepository<DoctorPayroll> _doctorPayrollRepository;
        private readonly IRepository<Appointment> _appointmentRepository;

        public DoctorPayrollService(
            IRepository<DoctorPayroll> doctorPayrollRepository,
            IRepository<Appointment> appointmentRepository)
        {
            _doctorPayrollRepository = doctorPayrollRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IEnumerable<DoctorPayroll>> GetAllAsync()
        {
            return await _doctorPayrollRepository.GetAsync(includeProperties: "Doctor");
        }

        public async Task<DoctorPayroll> GetByIdAsync(int id)
        {
            var p = await _doctorPayrollRepository.GetAsync(
                p => p.Id == id,
                includeProperties: "Doctor"
            );
            return p.FirstOrDefault();
        }

        public async Task<IEnumerable<DoctorPayrollViewModel>> GetAllByDoctorIdAsync(int doctorId)
        {
            var list = await _doctorPayrollRepository.GetAsync(
                p => p.DoctorId == doctorId,
                includeProperties: "Doctor"
            );

            var viewModels = new List<DoctorPayrollViewModel>();

            foreach (var payroll in list)
            {
                var appointments = await _appointmentRepository.GetAsync(
                    a => a.DoctorId == payroll.DoctorId &&
                        a.AppointmentDate.Month == payroll.Month &&
                        a.AppointmentDate.Year == payroll.Year &&
                        a.IsPaidByPatient &&
                        a.IsPaidToDoctor
                );

                viewModels.Add(new DoctorPayrollViewModel
                {
                    Id = payroll.Id,
                    DoctorId = payroll.DoctorId,
                    Month = payroll.Month,
                    Year = payroll.Year,
                    DoctorName = $"{payroll.Doctor?.FirstName} {payroll.Doctor?.LastName}",
                    TotalAppointments = appointments.Count(),
                    BaseSalary = payroll.BaseSalary,
                    Bonus = payroll.Bonus,
                    TotalSalary = payroll.TotalSalary,
                    Notes = payroll.Notes,
                    Status = payroll.Status,

                });
            }

            return viewModels;
        }


        public async Task CreateAsync(DoctorPayroll payroll)
        {
            payroll.TotalSalary = payroll.BaseSalary + payroll.Bonus;
            await _doctorPayrollRepository.AddAsync(payroll);
        }

        public async Task UpdateAsync(DoctorPayrollViewModel model)
        {
            var payroll = await _doctorPayrollRepository.GetByIdAsync(model.Id);
            if (payroll == null) return;

            payroll.BaseSalary = model.BaseSalary;
            payroll.Bonus = model.Bonus;
            payroll.TotalSalary = model.BaseSalary + model.Bonus;
            payroll.Notes = model.Notes;
            payroll.Status = model.Status;

            await _doctorPayrollRepository.UpdateAsync(payroll);
            await _doctorPayrollRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _doctorPayrollRepository.GetByIdAsync(id);
            if (entity != null)
            {
                await _doctorPayrollRepository.DeleteAsync(entity);
                await _doctorPayrollRepository.SaveAsync();
            }
        }

        public async Task CreatePayrollWithAutoAppointmentsAsync(DoctorPayroll payroll)
        {
            var exists = await _doctorPayrollRepository.GetFirstOrDefaultAsync(
                    p => p.DoctorId == payroll.DoctorId &&
                    p.Month == payroll.Month &&
                    p.Year == payroll.Year
            );

            if (exists != null)
                throw new Exception($"Bảng lương đã tồn tại cho tháng {exists.Month}/{exists.Year}.");
            
            var appointments = await _appointmentRepository.GetAsync(
                    a => a.DoctorId == payroll.DoctorId &&
                        a.AppointmentDate.Month == payroll.Month &&
                        a.AppointmentDate.Year == payroll.Year &&
                        a.IsPaidByPatient == true &&
                        a.IsPaidToDoctor == false
                );

            Console.WriteLine("OKKKKKKKKKKKKKKKKKKKK " + appointments.Count().ToString());
            payroll.TotalAppointments = appointments.Count();
            payroll.TotalSalary = payroll.BaseSalary + payroll.Bonus;
            payroll.CreatedAt = DateTime.Now;
            payroll.Status = "Pending";

            await _doctorPayrollRepository.AddAsync(payroll);

            foreach (var appt in appointments)
                appt.IsPaidToDoctor = true;

            await _appointmentRepository.SaveAsync();
        }
        
        public async Task<IEnumerable<Appointment>> GetAsync(Expression<Func<Appointment, bool>> filter)
        {
            return await _appointmentRepository.GetAsync(filter);
        }

    }
}
