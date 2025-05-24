using HospitalManagement.Data;
using HospitalManagement.Models.Entities;
using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AccountService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<IEnumerable<string>> GetRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Enumerable.Empty<string>();

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded) return false;

            switch (role)
            {
                case "Doctor":
                    var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                    if (existingDoctor == null)
                    {
                        _context.Doctors.Add(new Doctor
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            UserId = user.Id,
                            DepartmentId = 1,
                            Specialization = "Unknown",
                            ConsultationFee = 0,
                            Gender = "Unknown",
                            PhoneNumber = "Unknown",
                            IsActive = true
                        });
                    }
                    else
                    {
                        existingDoctor.IsActive = true;
                    }
                    break;

                case "Staff":
                    var existingStaff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == userId);
                    if (existingStaff == null)
                    {
                        _context.Staff.Add(new Staff
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            UserId = user.Id,
                            DepartmentId = 1,
                            Gender = "Unknown",
                            PhoneNumber = "Unknown",
                            Position = "Unknown",
                            IsActive = true
                        });
                    }
                    else
                    {
                        existingStaff.IsActive = true;
                    }
                    break;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded) return false;

            switch (role)
            {
                case "Doctor":
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                    if (doctor != null)
                    {
                        doctor.IsActive = false;
                    }
                    break;

                case "Staff":
                    var staff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == userId);
                    if (staff != null)
                    {
                        staff.IsActive = false;
                    }
                    break;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ApplicationUser>> GetAvailableDoctorUsersAsync()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var doctorUserIds = await _context.Doctors.Select(d => d.UserId).ToListAsync();

            var availableUsers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                if (doctorUserIds.Contains(user.Id)) continue;

                var roles = await _userManager.GetRolesAsync(user);

                if (!roles.Contains("Doctor"))
                    availableUsers.Add(user);
                else if (roles.Contains("Doctor") && !_context.Doctors.Any(d => d.UserId == user.Id))
                    availableUsers.Add(user);
            }

            return availableUsers;
        }

        public async Task<List<ApplicationUser>> GetAvailableStaffUsersAsync()
        {
            var assignedStaffIds = await _context.Staff.Select(s => s.UserId).ToListAsync();
            var allStaffUsers = await _userManager.GetUsersInRoleAsync("Staff");

            return allStaffUsers
                .Where(u => !assignedStaffIds.Contains(u.Id))
                .ToList();
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

    }
}
