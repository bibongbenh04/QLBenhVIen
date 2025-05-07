using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<HospitalManagement.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("HospitalManagement.Web")));

builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<HospitalManagement.Data.ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<HospitalManagement.Core.Services.Interfaces.IPatientService, HospitalManagement.Core.Services.PatientService>();
builder.Services.AddScoped<HospitalManagement.Core.Services.Interfaces.IDoctorService, HospitalManagement.Core.Services.DoctorService>();
builder.Services.AddScoped<HospitalManagement.Data.Repositories.IRepository<HospitalManagement.Models.Entities.Appointment>, HospitalManagement.Data.Repositories.Repository<HospitalManagement.Models.Entities.Appointment>>();
builder.Services.AddScoped<HospitalManagement.Data.Repositories.IRepository<HospitalManagement.Models.Entities.MedicalRecord>, HospitalManagement.Data.Repositories.Repository<HospitalManagement.Models.Entities.MedicalRecord>>();
builder.Services.AddScoped<HospitalManagement.Data.Repositories.IRepository<HospitalManagement.Models.Entities.Patient>, HospitalManagement.Data.Repositories.Repository<HospitalManagement.Models.Entities.Patient>>();
builder.Services.AddScoped<HospitalManagement.Data.Repositories.IRepository<HospitalManagement.Models.Entities.Bill>, HospitalManagement.Data.Repositories.Repository<HospitalManagement.Models.Entities.Bill>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
