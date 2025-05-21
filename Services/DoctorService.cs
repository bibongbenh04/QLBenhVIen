using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalManagement.Data;
using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IRepository<DoctorSchedule> _scheduleRepository;
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorService(
            IRepository<Doctor> doctorRepository,
            IRepository<DoctorSchedule> scheduleRepository,
            IRepository<Appointment> appointmentRepository,
            IRepository<Department> departmentRepository,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _doctorRepository = doctorRepository;
            _scheduleRepository = scheduleRepository;
            _appointmentRepository = appointmentRepository;
            _departmentRepository = departmentRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<DoctorViewModel>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAsync(d => d.IsActive, null, "Department");
            return doctors.Select(d => new DoctorViewModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Gender = d.Gender,
                PhoneNumber = d.PhoneNumber,
                Email = d.Email,
                Specialization = d.Specialization,
                Qualifications = d.Qualifications,
                Biography = d.Biography,
                ConsultationFee = d.ConsultationFee,
                DepartmentId = d.DepartmentId,
                DepartmentName = d.Department?.Name
            });
        }

        public async Task<DoctorViewModel> GetDoctorByIdAsync(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return null;

            return new DoctorViewModel
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Gender = doctor.Gender,
                PhoneNumber = doctor.PhoneNumber,
                Email = doctor.Email,
                Specialization = doctor.Specialization,
                Qualifications = doctor.Qualifications,
                Biography = doctor.Biography,
                ConsultationFee = doctor.ConsultationFee,
                DepartmentId = doctor.DepartmentId,
                DepartmentName = doctor.Department?.Name,
                UserId = doctor.UserId
            };
        }



        public async Task<DoctorDetailsViewModel> GetDoctorDetailsAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null) return null;

            var schedules = await _scheduleRepository.GetAsync(s => s.DoctorId == id);
            var appointments = await _appointmentRepository.GetAsync(a => a.DoctorId == id);

            return new DoctorDetailsViewModel
            {
                Doctor = await GetDoctorByIdAsync(id),
                Schedules = schedules.Select(s => new DoctorScheduleViewModel
                {
                    Id = s.Id,
                    DoctorId = s.DoctorId,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    MaxAppointments = s.MaxAppointments,
                    IsAvailable = s.IsAvailable
                }).ToList(),
                Appointments = appointments.Select(a => new AppointmentViewModels
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    PatientId = a.PatientId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Reason = a.Reason,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                }).ToList()
            };
        }

        public async Task<List<DoctorViewModel>> GetDoctorsByDepartmentIdAsync(int departmentId)
        {
            var doctors = await _doctorRepository.GetAsync(d => d.DepartmentId == departmentId && d.IsActive, includeProperties: "Department");
            return doctors.Select(d => new DoctorViewModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Gender = d.Gender,
                PhoneNumber = d.PhoneNumber,
                Email = d.Email,
                Specialization = d.Specialization,
                Qualifications = d.Qualifications,
                Biography = d.Biography,
                ConsultationFee = d.ConsultationFee,
                DepartmentId = d.DepartmentId,
                DepartmentName = d.Department?.Name
            }).ToList();
        }

        public async Task<Doctor> CreateDoctorAsync(DoctorViewModel model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("Tài khoản không tồn tại");

            var existing = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (existing != null) throw new Exception("Tài khoản đã được gán làm bác sĩ");

            var doctor = new Doctor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Specialization = model.Specialization,
                Qualifications = model.Qualifications,
                Biography = model.Biography,
                ConsultationFee = model.ConsultationFee,
                DepartmentId = model.DepartmentId,
                UserId = userId,
                IsActive = true
            };

            await _doctorRepository.AddAsync(doctor);
            await _doctorRepository.SaveAsync();

            if (!await _userManager.IsInRoleAsync(user, "Doctor"))
                await _userManager.AddToRoleAsync(user, "Doctor");

            return doctor;
        }

        public async Task UpdateDoctorAsync(DoctorViewModel model)
        {
            var doctor = await _doctorRepository.GetByIdAsync(model.Id);
            if (doctor == null) throw new Exception("Doctor not found");

            doctor.FirstName = model.FirstName;
            doctor.LastName = model.LastName;
            doctor.Gender = model.Gender;
            doctor.PhoneNumber = model.PhoneNumber;
            doctor.Email = model.Email;
            doctor.Specialization = model.Specialization;
            doctor.Qualifications = model.Qualifications;
            doctor.Biography = model.Biography;
            doctor.ConsultationFee = model.ConsultationFee;
            doctor.DepartmentId = model.DepartmentId;

            await _doctorRepository.UpdateAsync(doctor);
            await _doctorRepository.SaveAsync();
        }

        public async Task DeleteDoctorAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            await _doctorRepository.DeleteAsync(doctor);
            await _doctorRepository.SaveAsync();
        }

        public async Task<IEnumerable<DoctorScheduleViewModel>> GetDoctorSchedulesAsync(int doctorId)
        {
            var schedules = await _scheduleRepository.GetAsync(s => s.DoctorId == doctorId);
            return schedules.Select(s => new DoctorScheduleViewModel
            {
                Id = s.Id,
                DoctorId = s.DoctorId,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MaxAppointments = s.MaxAppointments,
                IsAvailable = s.IsAvailable
            });
        }

        public async Task UpdateDoctorScheduleAsync(DoctorScheduleViewModel model)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(model.Id);
            schedule.DoctorId = model.DoctorId;
            schedule.DayOfWeek = model.DayOfWeek;
            schedule.StartTime = model.StartTime;
            schedule.EndTime = model.EndTime;
            schedule.MaxAppointments = model.MaxAppointments;
            schedule.IsAvailable = model.IsAvailable;

            await _scheduleRepository.UpdateAsync(schedule);
            await _scheduleRepository.SaveAsync();
        }

        public async Task AddDoctorScheduleAsync(DoctorScheduleViewModel model)
        {
            var schedule = new DoctorSchedule
            {
                DoctorId = model.DoctorId,
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                MaxAppointments = model.MaxAppointments,
                IsAvailable = model.IsAvailable
            };

            await _scheduleRepository.AddAsync(schedule);
            await _scheduleRepository.SaveAsync();
        }

        public async Task<IEnumerable<DoctorViewModel>> GetPaginatedDoctorsAsync(int pageNumber, int pageSize)
        {
            var query = _doctorRepository.Query()
                .Include(d => d.Department)
                .Select(d => new DoctorViewModel
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    DepartmentName = d.Department.Name
                });

            return query;
        }

        public async Task<DoctorScheduleViewModel> GetScheduleByIdAsync(int id)
        {
            var s = await _scheduleRepository.GetByIdAsync(id);
            return s == null ? null : new DoctorScheduleViewModel
            {
                Id = s.Id,
                DoctorId = s.DoctorId,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MaxAppointments = s.MaxAppointments,
                IsAvailable = s.IsAvailable
            };
        }

        public async Task<int> DeleteDoctorScheduleAsync(int id)
        {
            var s = await _scheduleRepository.GetByIdAsync(id);
            if (s == null) return -1;
            int doctorId = s.DoctorId;
            await _scheduleRepository.DeleteAsync(s);
            await _scheduleRepository.SaveAsync();
            return doctorId;
        }
    }
}
