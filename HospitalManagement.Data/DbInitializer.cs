using HospitalManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if the database already has data
                if (context.Departments.Any())
                {
                    return; // Database has been seeded
                }

                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Create roles
                string[] roleNames = { "Admin", "Doctor", "Staff", "Patient" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Create admin user
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@hospital.com",
                    Email = "admin@hospital.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                await CreateUser(userManager, adminUser, "Admin@123", "Admin");

                // Create departments
                var departments = new List<Department>
                {
                    new Department { Name = "Cardiology", Description = "Heart and cardiovascular system" },
                    new Department { Name = "Neurology", Description = "Brain and nervous system" },
                    new Department { Name = "Orthopedics", Description = "Bones, joints, and muscles" },
                    new Department { Name = "Pediatrics", Description = "Children's health" },
                    new Department { Name = "Dermatology", Description = "Skin conditions" }
                };

                await context.Departments.AddRangeAsync(departments);
                await context.SaveChangesAsync();

                // Create doctors
                var doctorUsers = new List<(ApplicationUser User, string Password, Doctor Doctor)>
                {
                    (
                        new ApplicationUser
                        {
                            UserName = "doctor1@hospital.com",
                            Email = "doctor1@hospital.com",
                            FirstName = "John",
                            LastName = "Smith",
                            EmailConfirmed = true,
                            CreatedAt = DateTime.Now,
                            IsActive = true
                        },
                        "Doctor@123",
                        new Doctor
                        {
                            FirstName = "John",
                            LastName = "Smith",
                            Gender = "Male",
                            PhoneNumber = "1234567890",
                            Email = "doctor1@hospital.com",
                            Specialization = "Cardiologist",
                            Qualifications = "MD, PhD",
                            Biography = "Experienced cardiologist with over 10 years of practice.",
                            ConsultationFee = 150.00M,
                            DepartmentId = 1
                        }
                    ),
                    (
                        new ApplicationUser
                        {
                            UserName = "doctor2@hospital.com",
                            Email = "doctor2@hospital.com",
                            FirstName = "Emily",
                            LastName = "Johnson",
                            EmailConfirmed = true,
                            CreatedAt = DateTime.Now,
                            IsActive = true
                        },
                        "Doctor@123",
                        new Doctor
                        {
                            FirstName = "Emily",
                            LastName = "Johnson",
                            Gender = "Female",
                            PhoneNumber = "2345678901",
                            Email = "doctor2@hospital.com",
                            Specialization = "Neurologist",
                            Qualifications = "MD",
                            Biography = "Specialized in treating neurological disorders.",
                            ConsultationFee = 180.00M,
                            DepartmentId = 2
                        }
                    ),
                    (
                        new ApplicationUser
                        {
                            UserName = "doctor3@hospital.com",
                            Email = "doctor3@hospital.com",
                            FirstName = "Michael",
                            LastName = "Brown",
                            EmailConfirmed = true,
                            CreatedAt = DateTime.Now,
                            IsActive = true
                        },
                        "Doctor@123",
                        new Doctor
                        {
                            FirstName = "Michael",
                            LastName = "Brown",
                            Gender = "Male",
                            PhoneNumber = "3456789012",
                            Email = "doctor3@hospital.com",
                            Specialization = "Orthopedic Surgeon",
                            Qualifications = "MD, MS",
                            Biography = "Expert in joint replacement surgeries.",
                            ConsultationFee = 200.00M,
                            DepartmentId = 3
                        }
                    )
                };

                foreach (var doctorData in doctorUsers)
                {
                    var user = await CreateUser(userManager, doctorData.User, doctorData.Password, "Doctor");
                    var doctor = doctorData.Doctor;
                    doctor.UserId = user.Id;
                    await context.Doctors.AddAsync(doctor);
                }

                await context.SaveChangesAsync();

                // Create doctor schedules
                var doctorSchedules = new List<DoctorSchedule>();
                var doctors = await context.Doctors.ToListAsync();

                foreach (var doctor in doctors)
                {
                    // Create schedules for weekdays
                    for (int i = 1; i <= 5; i++) // Monday to Friday
                    {
                        doctorSchedules.Add(new DoctorSchedule
                        {
                            DoctorId = doctor.Id,
                            DayOfWeek = (DayOfWeek)i,
                            StartTime = new TimeSpan(9, 0, 0), // 9:00 AM
                            EndTime = new TimeSpan(17, 0, 0),  // 5:00 PM
                            MaxAppointments = 10,
                            IsAvailable = true
                        });
                    }
                }

                await context.DoctorSchedules.AddRangeAsync(doctorSchedules);
                await context.SaveChangesAsync();

                // Create staff
                var staffUser = new ApplicationUser
                {
                    UserName = "staff1@hospital.com",
                    Email = "staff1@hospital.com",
                    FirstName = "Sarah",
                    LastName = "Wilson",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                var staff = new Staff
                {
                    FirstName = "Sarah",
                    LastName = "Wilson",
                    Gender = "Female",
                    PhoneNumber = "4567890123",
                    Email = "staff1@hospital.com",
                    Position = "Receptionist",
                    JoiningDate = DateTime.Now.AddYears(-2)
                };

                var staffUserCreated = await CreateUser(userManager, staffUser, "Staff@123", "Staff");
                staff.UserId = staffUserCreated.Id;
                await context.Staff.AddAsync(staff);
                await context.SaveChangesAsync();

                // Create patients
                var patientUsers = new List<(ApplicationUser User, string Password, Patient Patient)>
                {
                    (
                        new ApplicationUser
                        {
                            UserName = "patient1@example.com",
                            Email = "patient1@example.com",
                            FirstName = "Robert",
                            LastName = "Davis",
                            EmailConfirmed = true,
                            CreatedAt = DateTime.Now,
                            IsActive = true
                        },
                        "Patient@123",
                        new Patient
                        {
                            FirstName = "Robert",
                            LastName = "Davis",
                            DateOfBirth = new DateTime(1985, 5, 15),
                            Gender = "Male",
                            PhoneNumber = "5678901234",
                            Email = "patient1@example.com",
                            Address = "123 Main St, Anytown",
                            EmergencyContactName = "Mary Davis",
                            EmergencyContactPhone = "5678901235",
                            BloodGroup = "A+",
                            HasInsurance = true,
                            InsuranceProvider = "Health Insurance Co",
                            InsurancePolicyNumber = "HI12345678"
                        }
                    ),
                    (
                        new ApplicationUser
                        {
                            UserName = "patient2@example.com",
                            Email = "patient2@example.com",
                            FirstName = "Jennifer",
                            LastName = "Lee",
                            EmailConfirmed = true,
                            CreatedAt = DateTime.Now,
                            IsActive = true
                        },
                        "Patient@123",
                        new Patient
                        {
                            FirstName = "Jennifer",
                            LastName = "Lee",
                            DateOfBirth = new DateTime(1990, 8, 22),
                            Gender = "Female",
                            PhoneNumber = "6789012345",
                            Email = "patient2@example.com",
                            Address = "456 Oak Ave, Somewhere",
                            EmergencyContactName = "David Lee",
                            EmergencyContactPhone = "6789012346",
                            BloodGroup = "B-",
                            HasInsurance = true,
                            InsuranceProvider = "Medical Insurance Inc",
                            InsurancePolicyNumber = "MI98765432"
                        }
                    ),
                    (
                        new ApplicationUser
                        {
                            UserName = "patient3@example.com",
                            Email = "patient3@example.com",
                            FirstName = "William",
                            LastName = "Taylor",
                            EmailConfirmed = true,
                            CreatedAt = DateTime.Now,
                            IsActive = true
                        },
                        "Patient@123",
                        new Patient
                        {
                            FirstName = "William",
                            LastName = "Taylor",
                            DateOfBirth = new DateTime(1978, 3, 10),
                            Gender = "Male",
                            PhoneNumber = "7890123456",
                            Email = "patient3@example.com",
                            Address = "789 Pine Rd, Elsewhere",
                            EmergencyContactName = "Susan Taylor",
                            EmergencyContactPhone = "7890123457",
                            BloodGroup = "O+",
                            HasInsurance = false
                        }
                    ),
                    (
                        new ApplicationUser
                        {
                            UserName = "patient4@example.com",
                            Email = "patient4@example.com",
                            FirstName = "Patricia",
                            LastName = "Martinez",
                            EmailConfirmed = true,
                            CreatedAt = DateTime.Now,
                            IsActive = true
                        },
                        "Patient@123",
                        new Patient
                        {
                            FirstName = "Patricia",
                            LastName = "Martinez",
                            DateOfBirth = new DateTime(1995, 11, 28),
                            Gender = "Female",
                            PhoneNumber = "8901234567",
                            Email = "patient4@example.com",
                            Address = "101 Maple Dr, Nowhere",
                            EmergencyContactName = "Carlos Martinez",
                            EmergencyContactPhone = "8901234568",
                            BloodGroup = "AB+",
                            HasInsurance = true,
                            InsuranceProvider = "National Health Insurance",
                            InsurancePolicyNumber = "NH24681357"
                        }
                    ),
                    (
                        new ApplicationUser
                        {
                            UserName = "patient5@example.com",
                            Email = "patient5@example.com",
                            FirstName = "James",
                            LastName = "Wilson",
                            EmailConfirmed = true,
                            CreatedAt = DateTime.Now,
                            IsActive = true
                        },
                        "Patient@123",
                        new Patient
                        {
                            FirstName = "James",
                            LastName = "Wilson",
                            DateOfBirth = new DateTime(1982, 7, 4),
                            Gender = "Male",
                            PhoneNumber = "9012345678",
                            Email = "patient5@example.com",
                            Address = "202 Cedar Ln, Anyplace",
                            EmergencyContactName = "Elizabeth Wilson",
                            EmergencyContactPhone = "9012345679",
                            BloodGroup = "A-",
                            HasInsurance = true,
                            InsuranceProvider = "Global Health Care",
                            InsurancePolicyNumber = "GH13579246"
                        }
                    )
                };

                foreach (var patientData in patientUsers)
                {
                    var user = await CreateUser(userManager, patientData.User, patientData.Password, "Patient");
                    var patient = patientData.Patient;
                    patient.UserId = user.Id;
                    await context.Patients.AddAsync(patient);
                }

                await context.SaveChangesAsync();

                // Create medications
                var medications = new List<Medication>
                {
                    new Medication
                    {
                        Name = "Aspirin",
                        GenericName = "Acetylsalicylic acid",
                        Brand = "Bayer",
                        Category = "Pain Reliever",
                        Description = "Used to treat pain, fever, and inflammation.",
                        DosageForm = "Tablet",
                        Strength = "325 mg",
                        UnitPrice = 0.15M,
                        StockQuantity = 1000,
                        ReorderLevel = 200
                    },
                    new Medication
                    {
                        Name = "Lisinopril",
                        GenericName = "Lisinopril",
                        Brand = "Prinivil",
                        Category = "ACE Inhibitor",
                        Description = "Used to treat high blood pressure and heart failure.",
                        DosageForm = "Tablet",
                        Strength = "10 mg",
                        UnitPrice = 0.50M,
                        StockQuantity = 800,
                        ReorderLevel = 150
                    },
                    new Medication
                    {
                        Name = "Metformin",
                        GenericName = "Metformin Hydrochloride",
                        Brand = "Glucophage",
                        Category = "Antidiabetic",
                        Description = "Used to treat type 2 diabetes.",
                        DosageForm = "Tablet",
                        Strength = "500 mg",
                        UnitPrice = 0.25M,
                        StockQuantity = 1200,
                        ReorderLevel = 250
                    },
                    new Medication
                    {
                        Name = "Atorvastatin",
                        GenericName = "Atorvastatin Calcium",
                        Brand = "Lipitor",
                        Category = "Statin",
                        Description = "Used to lower cholesterol levels.",
                        DosageForm = "Tablet",
                        Strength = "20 mg",
                        UnitPrice = 1.20M,
                        StockQuantity = 600,
                        ReorderLevel = 120
                    },
                    new Medication
                    {
                        Name = "Amoxicillin",
                        GenericName = "Amoxicillin Trihydrate",
                        Brand = "Amoxil",
                        Category = "Antibiotic",
                        Description = "Used to treat bacterial infections.",
                        DosageForm = "Capsule",
                        Strength = "500 mg",
                        UnitPrice = 0.75M,
                        StockQuantity = 900,
                        ReorderLevel = 180
                    },
                    new Medication
                    {
                        Name = "Levothyroxine",
                        GenericName = "Levothyroxine Sodium",
                        Brand = "Synthroid",
                        Category = "Thyroid Hormone",
                        Description = "Used to treat hypothyroidism.",
                        DosageForm = "Tablet",
                        Strength = "50 mcg",
                        UnitPrice = 0.60M,
                        StockQuantity = 700,
                        ReorderLevel = 140
                    },
                    new Medication
                    {
                        Name = "Ibuprofen",
                        GenericName = "Ibuprofen",
                        Brand = "Advil",
                        Category = "NSAID",
                        Description = "Used to treat pain, fever, and inflammation.",
                        DosageForm = "Tablet",
                        Strength = "200 mg",
                        UnitPrice = 0.20M,
                        StockQuantity = 1500,
                        ReorderLevel = 300
                    },
                    new Medication
                    {
                        Name = "Omeprazole",
                        GenericName = "Omeprazole",
                        Brand = "Prilosec",
                        Category = "Proton Pump Inhibitor",
                        Description = "Used to treat acid reflux and ulcers.",
                        DosageForm = "Capsule",
                        Strength = "20 mg",
                        UnitPrice = 0.80M,
                        StockQuantity = 500,
                        ReorderLevel = 100
                    },
                    new Medication
                    {
                        Name = "Amlodipine",
                        GenericName = "Amlodipine Besylate",
                        Brand = "Norvasc",
                        Category = "Calcium Channel Blocker",
                        Description = "Used to treat high blood pressure and angina.",
                        DosageForm = "Tablet",
                        Strength = "5 mg",
                        UnitPrice = 0.45M,
                        StockQuantity = 850,
                        ReorderLevel = 170
                    },
                    new Medication
                    {
                        Name = "Sertraline",
                        GenericName = "Sertraline Hydrochloride",
                        Brand = "Zoloft",
                        Category = "SSRI",
                        Description = "Used to treat depression and anxiety disorders.",
                        DosageForm = "Tablet",
                        Strength = "50 mg",
                        UnitPrice = 0.90M,
                        StockQuantity = 400,
                        ReorderLevel = 80
                    }
                };

                await context.Medications.AddRangeAsync(medications);
                await context.SaveChangesAsync();

                // Create some appointments
                var patients = await context.Patients.ToListAsync();
                var random = new Random();

                var appointments = new List<Appointment>();
                var startDate = DateTime.Now.AddDays(-30);
                var endDate = DateTime.Now.AddDays(30);

                for (int i = 0; i < 20; i++)
                {
                    var appointmentDate = startDate.AddDays(random.Next((endDate - startDate).Days));
                    var appointmentTime = new TimeSpan(
                        random.Next(9, 17), // Hours between 9 AM and 5 PM
                        random.Next(0, 4) * 15, // Minutes (0, 15, 30, 45)
                        0 // Seconds
                    );

                    var patientId = patients[random.Next(patients.Count)].Id;
                    var doctorId = doctors[random.Next(doctors.Count)].Id;

                    appointments.Add(new Appointment
                    {
                        PatientId = patientId,
                        DoctorId = doctorId,
                        AppointmentDate = appointmentDate,
                        AppointmentTime = appointmentTime,
                        Reason = "Regular checkup",
                        Status = appointmentDate < DateTime.Now ? "Completed" : "Scheduled",
                        CreatedAt = DateTime.Now.AddDays(-random.Next(1, 60))
                    });
                }

                await context.Appointments.AddRangeAsync(appointments);
                await context.SaveChangesAsync();

                // Create medical records for completed appointments
                var completedAppointments = await context.Appointments
                    .Where(a => a.Status == "Completed")
                    .ToListAsync();

                var medicalRecords = new List<MedicalRecord>();
                var diagnoses = new[]
                {
                    "Common Cold", "Hypertension", "Type 2 Diabetes", "Influenza",
                    "Bronchitis", "Migraine", "Gastritis", "Allergic Rhinitis"
                };

                var treatments = new[]
                {
                    "Rest and hydration", "Prescribed medication", "Lifestyle changes",
                    "Physical therapy", "Regular monitoring", "Dietary changes"
                };

                foreach (var appointment in completedAppointments)
                {
                    medicalRecords.Add(new MedicalRecord
                    {
                        PatientId = appointment.PatientId,
                        AppointmentId = appointment.Id,
                        Diagnosis = diagnoses[random.Next(diagnoses.Length)],
                        Treatment = treatments[random.Next(treatments.Length)],
                        Notes = "Patient responded well to treatment.",
                        RecordDate = appointment.AppointmentDate
                    });
                }

                await context.MedicalRecords.AddRangeAsync(medicalRecords);
                await context.SaveChangesAsync();

                // Create prescriptions for some medical records
                var allMedicalRecords = await context.MedicalRecords.ToListAsync();
                var allMedications = await context.Medications.ToListAsync();

                var prescriptions = new List<Prescription>();
                var prescriptionItems = new List<PrescriptionItem>();

                foreach (var record in allMedicalRecords)
                {
                    var prescription = new Prescription
                    {
                        MedicalRecordId = record.Id,
                        DoctorId = doctors[random.Next(doctors.Count)].Id,
                        PrescriptionDate = record.RecordDate,
                        Notes = "Take medications as prescribed."
                    };

                    prescriptions.Add(prescription);
                }

                await context.Prescriptions.AddRangeAsync(prescriptions);
                await context.SaveChangesAsync();

                // Add prescription items
                foreach (var prescription in prescriptions)
                {
                    // Add 1-3 medications to each prescription
                    int medicationCount = random.Next(1, 4);
                    var selectedMedicationIds = new HashSet<int>();

                    for (int i = 0; i < medicationCount; i++)
                    {
                        int medicationId;
                        do
                        {
                            medicationId = allMedications[random.Next(allMedications.Count)].Id;
                        } while (selectedMedicationIds.Contains(medicationId));

                        selectedMedicationIds.Add(medicationId);

                        prescriptionItems.Add(new PrescriptionItem
                        {
                            PrescriptionId = prescription.Id,
                            MedicationId = medicationId,
                            Dosage = $"{random.Next(1, 3)} tablet(s)",
                            Instructions = $"{random.Next(1, 4)} time(s) daily after meals",
                            Quantity = random.Next(10, 31),
                            DurationDays = random.Next(5, 15)
                        });
                    }
                }

                await context.PrescriptionItems.AddRangeAsync(prescriptionItems);
                await context.SaveChangesAsync();

                // Create bills for completed appointments
                var bills = new List<Bill>();
                var billItems = new List<BillItem>();

                foreach (var appointment in completedAppointments)
                {
                    var doctor = await context.Doctors.FindAsync(appointment.DoctorId);
                    var patient = await context.Patients.FindAsync(appointment.PatientId);

                    var consultationFee = doctor.ConsultationFee;
                    var totalAmount = consultationFee;

                    // Check if there's a medical record
                    var medicalRecord = await context.MedicalRecords
                        .FirstOrDefaultAsync(m => m.AppointmentId == appointment.Id);

                    // Add medication costs if there's a prescription
                    decimal medicationCost = 0;
                    if (medicalRecord != null)
                    {
                        var prescription = await context.Prescriptions
                            .FirstOrDefaultAsync(p => p.MedicalRecordId == medicalRecord.Id);

                        if (prescription != null)
                        {
                            var items = await context.PrescriptionItems
                                .Where(pi => pi.PrescriptionId == prescription.Id)
                                .Include(pi => pi.Medication)
                                .ToListAsync();

                            foreach (var item in items)
                            {
                                medicationCost += item.Medication.UnitPrice * item.Quantity;
                            }
                        }
                    }

                    totalAmount += medicationCost;

                    // Calculate insurance coverage if applicable
                    decimal insuranceCoverage = 0;
                    if (patient.HasInsurance)
                    {
                        // Assume insurance covers 80% of the total
                        insuranceCoverage = totalAmount * 0.8M;
                    }

                    var dueAmount = totalAmount - insuranceCoverage;

                    var bill = new Bill
                    {
                        PatientId = appointment.PatientId,
                        BillDate = appointment.AppointmentDate,
                        TotalAmount = totalAmount,
                        PaidAmount = dueAmount, // Assume bill is paid
                        DueAmount = 0,
                        InsuranceCoverage = insuranceCoverage,
                        PaymentStatus = "Paid",
                        PaymentMethod = "Credit Card"
                    };

                    bills.Add(bill);
                }

                await context.Bills.AddRangeAsync(bills);
                await context.SaveChangesAsync();

                // Add bill items
                foreach (var bill in bills)
                {
                    var appointment = completedAppointments
                        .FirstOrDefault(a => a.PatientId == bill.PatientId && a.AppointmentDate.Date == bill.BillDate.Date);

                    if (appointment != null)
                    {
                        var doctor = await context.Doctors.FindAsync(appointment.DoctorId);

                        // Add consultation fee
                        billItems.Add(new BillItem
                        {
                            BillId = bill.Id,
                            ItemName = "Consultation Fee",
                            ItemType = "Consultation",
                            Quantity = 1,
                            UnitPrice = doctor.ConsultationFee,
                            Subtotal = doctor.ConsultationFee
                        });

                        // Check if there's a medical record with prescription
                        var medicalRecord = await context.MedicalRecords
                            .FirstOrDefaultAsync(m => m.AppointmentId == appointment.Id);

                        if (medicalRecord != null)
                        {
                            var prescription = await context.Prescriptions
                                .FirstOrDefaultAsync(p => p.MedicalRecordId == medicalRecord.Id);

                            if (prescription != null)
                            {
                                var items = await context.PrescriptionItems
                                    .Where(pi => pi.PrescriptionId == prescription.Id)
                                    .Include(pi => pi.Medication)
                                    .ToListAsync();

                                foreach (var item in items)
                                {
                                    billItems.Add(new BillItem
                                    {
                                        BillId = bill.Id,
                                        ItemName = item.Medication.Name,
                                        ItemType = "Medication",
                                        Quantity = item.Quantity,
                                        UnitPrice = item.Medication.UnitPrice,
                                        Subtotal = item.Medication.UnitPrice * item.Quantity
                                    });
                                }
                            }
                        }
                    }
                }

                await context.BillItems.AddRangeAsync(billItems);
                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser> CreateUser(UserManager<ApplicationUser> userManager, ApplicationUser user, string password, string role)
        {
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                return user;
            }

            throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
