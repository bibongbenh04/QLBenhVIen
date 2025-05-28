using HospitalManagement.Services.Interfaces;
using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;

namespace HospitalManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IEmailSenderService _emailSender;


        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IPatientService patientService,
            IDoctorService doctorService,
            RoleManager<IdentityRole> roleManager,
            IEmailSenderService emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _patientService = patientService;
            _doctorService = doctorService;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Email chưa được xác nhận.");
                return View(model);
            }

            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var _user = await _userManager.FindByEmailAsync(model.Email);
                    _user.LastLogin = DateTime.Now;
                    await _userManager.UpdateAsync(_user);

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.AllRoles = new List<string> { "Admin", "Doctor", "Staff" };
            return View();
        }


        // [HttpPost]
        // [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl)
        // {
        //     ViewData["ReturnUrl"] = returnUrl;
        //     ViewBag.AllRoles = new List<string> { "Admin", "Doctor", "Staff" };

        //     if (!ModelState.IsValid)
        //     {
        //         return View(model);
        //     }

        //     var user = new ApplicationUser
        //     {
        //         UserName = model.Email,
        //         Email = model.Email,
        //         FirstName = model.FirstName,
        //         LastName = model.LastName,
        //         CreatedAt = DateTime.Now,
        //         IsActive = true
        //     };

        //     var result = await _userManager.CreateAsync(user, model.Password);
        //     if (result.Succeeded)
        //     {
        //         await _userManager.AddToRolesAsync(user, model.Roles);

        //         if (model.Roles.Contains("Doctor"))
        //         {
        //             await _doctorService.CreateDoctorAsync(new DoctorViewModel
        //             {
        //                 FirstName = model.FirstName,
        //                 LastName = model.LastName,
        //                 Email = model.Email,
        //                 DepartmentId = 1
        //             }, user.Id);
        //         }

        //         await _signInManager.SignInAsync(user, isPersistent: false);
        //         return RedirectToLocal(returnUrl);
        //     }

        //     AddErrors(result);
        //     return View(model);
        // }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.AllRoles = new List<string> { "Admin", "Doctor", "Staff" };

            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, model.Roles);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Xác nhận tài khoản",
                    $"Vui lòng <a href='{confirmationLink}'>click vào đây để xác nhận</a>.");

                TempData["Info"] = "Vui lòng kiểm tra email để xác nhận tài khoản.";
                return RedirectToAction(nameof(Login));
            }

            AddErrors(result);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return View("ConfirmEmailSuccess");

            return View("ConfirmEmailFailed");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ApplicationUser model, IFormFile avatarFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;

            // Upload avatar
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(avatarFile.FileName)}";
                var path = Path.Combine("wwwroot/uploads/avatars", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                user.AvatarUrl = $"/uploads/avatars/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Thông tin đã được cập nhật.";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View("Profile", user);
        }



        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            AddErrors(result);
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            // Redirect về trang chính với thông báo SweetAlert
            return RedirectToAction("Index", "Home", new { accessDenied = true });
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                TempData["Error"] = $"Lỗi từ nhà cung cấp: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
                return RedirectToLocal(returnUrl);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                TempData["Error"] = "Không lấy được email từ Google.";
                return RedirectToAction(nameof(Login));
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "",
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "",
                IsActive = true
            };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);


                await _signInManager.SignInAsync(user, false);
                return RedirectToLocal(returnUrl);
            }

            AddErrors(identityResult);
            return RedirectToAction(nameof(Login));
        }

    }
}
