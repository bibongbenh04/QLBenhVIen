using HospitalManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace HospitalManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountManagementController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountManagementController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // public async Task<IActionResult> Index(int? page)
        // {
        //     int pageNumber = page ?? 1;
        //     int pageSize = 5;


        //     var users = await _accountService.GetAllUsersAsync();
        //     var pagedList = users.ToPagedList(pageNumber, pageSize);
        //     return View(pagedList);
        // }
        
        public async Task<IActionResult> Index(int? page, string keyword)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            var users = await _accountService.GetAllUsersAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                users = users.Where(u =>
                    (!string.IsNullOrEmpty(u.FirstName) && u.FirstName.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.LastName) && u.LastName.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(keyword))
                );
            }

            var pagedList = users.ToPagedList(pageNumber, pageSize);
            ViewBag.Keyword = keyword;
            return View(pagedList);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = new List<string> { "Admin", "Doctor", "Staff" };
            var userRoles = await _accountService.GetRolesAsync(user.Id);

            ViewBag.AllRoles = allRoles;
            ViewBag.UserRoles = userRoles;

            var claims = await _accountService.GetClaimsAsync(user.Id);
            var staffPosition = claims.FirstOrDefault(c => c.Type == "Position")?.Value;

            ViewBag.AllRoles = allRoles;
            ViewBag.UserRoles = userRoles;
            ViewBag.StaffPosition = staffPosition;


            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, List<string> roles)
        {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _accountService.GetRolesAsync(user.Id);
            foreach (var role in currentRoles)
                await _accountService.RemoveRoleAsync(user.Id, role);

            foreach (var role in roles)
                await _accountService.AssignRoleAsync(user.Id, role);

            if (roles.Contains("Staff"))
            {
                var staffPosition = Request.Form["staffPosition"].ToString();
                if (!string.IsNullOrEmpty(staffPosition))
                {
                    await _accountService.AssignClaimAsync(user.Id, "Position", staffPosition);
                }
            }
            else
            {
                await _accountService.AssignClaimAsync(user.Id, "Position", string.Empty);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _accountService.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }
    }
}
