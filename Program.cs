using HospitalManagement.Services;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Data;
using HospitalManagement.Data.Repositories;
using HospitalManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity setup
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Authentication
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsReceptionist", policy =>
        policy.RequireClaim("Position", "TiepTan"));

    options.AddPolicy("IsAccountant", policy =>
        policy.RequireClaim("Position", "KeToan"));

    options.AddPolicy("IsCashier", policy =>
        policy.RequireClaim("Position", "TaiVu"));

    options.AddPolicy("IsPharmacist", policy =>
        policy.RequireClaim("Position", "BanThuoc"));

    options.AddPolicy("IsManager", policy =>
        policy.RequireClaim("Position", "QuanLyChuyenMon"));

    options.AddPolicy("IsResourceManager", policy =>
        policy.RequireClaim("Position", "QuanLyTaiNguyen"));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DoctorsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim("Position", "QuanLyChuyenMon")
    ));

    options.AddPolicy("StaffsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim("Position", "QuanLyChuyenMon") ||
        context.User.HasClaim("Position", "QuanLyTaiNguyen")
    ));

    options.AddPolicy("DoctorPayrollsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim("Position", "KeToan") ||
        context.User.HasClaim("Position", "QuanLyChuyenMon")
    ));

    options.AddPolicy("StaffPayrollsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim("Position", "KeToan") ||
        context.User.HasClaim("Position", "QuanLyChuyenMon")
    ));

    options.AddPolicy("DepartmentsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim("Position", "QuanLyTaiNguyen")
    ));

    options.AddPolicy("MedicationsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim("Position", "BanThuoc") ||
        context.User.HasClaim("Position", "QuanLyChuyenMon") ||
        context.User.HasClaim("Position", "QuanLyTaiNguyen")
    ));

    options.AddPolicy("ServicesControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim("Position", "QuanLyChuyenMon") ||
        context.User.HasClaim("Position", "QuanLyTaiNguyen")
    ));

    options.AddPolicy("PatientsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim("Position", "TiepTan")
    ));

    options.AddPolicy("AppointmentsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim("Position", "TiepTan")
    ));

    options.AddPolicy("MedicalRecordsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.IsInRole("Doctor") ||
        context.User.HasClaim("Position", "TiepTan") || 
        context.User.HasClaim("Position", "QuanLyChuyenMon")
    ));

    options.AddPolicy("TestsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.IsInRole("Doctor") ||
        context.User.HasClaim("Position", "TiepTan") ||
        context.User.HasClaim("Position", "QuanLyChuyenMon")
    ));

    options.AddPolicy("PrescriptionsControllerAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.IsInRole("Doctor") ||
        context.User.HasClaim("Position", "BanThuoc") ||
        context.User.HasClaim("Position", "QuanLyChuyenMon")
    ));

    options.AddPolicy("BillingControllerAccess", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("Position", "KeToan") ||
            context.User.HasClaim("Position", "TaiVu") ||
            context.User.HasClaim("Position", "QuanLyTaiNguyen") ||
            context.User.HasClaim("Position", "QuanLyChuyenMon")
        ));
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
    options.AccessDeniedPath = ""; // <-- xóa mặc định
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


var app = builder.Build();

// Error handling
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
