using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HospitalManagement.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<List<ApplicationUser>> GetAvailableDoctorUsersAsync();

        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<string>> GetRolesAsync(string userId);
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<bool> RemoveRoleAsync(string userId, string role);

        Task<List<ApplicationUser>> GetAvailableStaffUsersAsync();
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task AssignClaimAsync(string userId, string claimType, string claimValue);
        Task<IList<Claim>> GetClaimsAsync(string userId);
    }
}
