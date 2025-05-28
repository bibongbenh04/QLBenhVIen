using HospitalManagement.Services;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Data;
using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity setup
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Google external login
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddGoogle(googleOptions =>
{
    IConfigurationSection googleAuthNSection =
        builder.Configuration.GetSection("Authentication:Google");

    googleOptions.ClientId = googleAuthNSection["ClientId"];
    googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
    googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = ""; 
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"message\":\"Bạn không có quyền truy cập.\"}");
            }

            context.Response.Redirect("/Home/Index?accessDenied=true");
            return Task.CompletedTask;
        }
    };
});


// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsReceptionist", p => p.RequireClaim("Position", "TiepTan"));
    options.AddPolicy("IsAccountant", p => p.RequireClaim("Position", "KeToan"));
    options.AddPolicy("IsCashier", p => p.RequireClaim("Position", "TaiVu"));
    options.AddPolicy("IsPharmacist", p => p.RequireClaim("Position", "BanThuoc"));
    options.AddPolicy("IsManager", p => p.RequireClaim("Position", "QuanLyChuyenMon"));
    options.AddPolicy("IsResourceManager", p => p.RequireClaim("Position", "QuanLyTaiNguyen"));

    options.AddPolicy("DoctorsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") || ctx.User.HasClaim("Position", "QuanLyChuyenMon")));

    options.AddPolicy("StaffsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.HasClaim("Position", "QuanLyChuyenMon") ||
                              ctx.User.HasClaim("Position", "QuanLyTaiNguyen")));

    options.AddPolicy("DoctorPayrollsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.HasClaim("Position", "KeToan") ||
                              ctx.User.HasClaim("Position", "QuanLyChuyenMon")));

    options.AddPolicy("StaffPayrollsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.HasClaim("Position", "KeToan") ||
                              ctx.User.HasClaim("Position", "QuanLyChuyenMon")));

    options.AddPolicy("DepartmentsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.HasClaim("Position", "QuanLyTaiNguyen")));

    options.AddPolicy("MedicationsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.HasClaim("Position", "BanThuoc") ||
                              ctx.User.HasClaim("Position", "QuanLyChuyenMon") ||
                              ctx.User.HasClaim("Position", "QuanLyTaiNguyen")));

    options.AddPolicy("ServicesControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.HasClaim("Position", "QuanLyChuyenMon") ||
                              ctx.User.HasClaim("Position", "QuanLyTaiNguyen")));

    options.AddPolicy("PatientsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.HasClaim("Position", "TiepTan")));

    options.AddPolicy("AppointmentsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.HasClaim("Position", "TiepTan")));

    options.AddPolicy("MedicalRecordsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.IsInRole("Doctor") ||
                              ctx.User.HasClaim("Position", "TiepTan") ||
                              ctx.User.HasClaim("Position", "QuanLyChuyenMon")));

    options.AddPolicy("TestsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.IsInRole("Doctor") ||
                              ctx.User.HasClaim("Position", "TiepTan") ||
                              ctx.User.HasClaim("Position", "QuanLyChuyenMon")));

    options.AddPolicy("PrescriptionsControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.IsInRole("Doctor") ||
                              ctx.User.HasClaim("Position", "BanThuoc") ||
                              ctx.User.HasClaim("Position", "QuanLyChuyenMon")));

    options.AddPolicy("BillingControllerAccess", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole("Admin") ||
                              ctx.User.HasClaim("Position", "TiepTan") ||
                              ctx.User.HasClaim("Position", "KeToan") ||
                              ctx.User.HasClaim("Position", "TaiVu") ||
                              ctx.User.HasClaim("Position", "QuanLyTaiNguyen") ||
                              ctx.User.HasClaim("Position", "QuanLyChuyenMon")));
});


// Identity config
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = ""; 
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToAccessDenied = context =>
        {
            // Check if it's an AJAX call or fetch (client expects JSON)
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"message\":\"Bạn không có quyền truy cập.\"}");
            }

            // Fallback: still allow redirect for non-AJAX
            context.Response.Redirect("/Home/Index?accessDenied=true");
            return Task.CompletedTask;
        }
    };
});




// TempData config using session
builder.Services.AddSession();
builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.Name = "TEMPDATA";
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddRazorPages();

// Dependency injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDoctorPayrollService, DoctorPayrollService>();
builder.Services.AddScoped<IStaffPayrollService, StaffPayrollService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
