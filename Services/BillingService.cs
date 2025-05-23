using HospitalManagement.Models.Entities;
using HospitalManagement.Models.ViewModels;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class BillingService : IBillingService
    {
        private readonly IRepository<Bill> _billRepo;
        private readonly IRepository<BillItem> _billItemRepo;
        private readonly IRepository<Patient> _patientRepo;
        private readonly IRepository<MedicalRecord> _medicalRecordRepo;
        private readonly IRepository<Appointment> _appointmentRepo;
        private readonly IRepository<Prescription> _prescriptionRepo;
        private readonly IRepository<PrescriptionItem> _prescriptionItemRepo;
        private readonly IRepository<Medication> _medicationRepo;
        private readonly IRepository<Test> _testRepo;

        public BillingService(
            IRepository<Bill> billRepo,
            IRepository<BillItem> billItemRepo,
            IRepository<Patient> patientRepo,
            IRepository<MedicalRecord> medicalRecordRepo,
            IRepository<Appointment> appointmentRepo,
            IRepository<Prescription> prescriptionRepo,
            IRepository<PrescriptionItem> prescriptionItemRepo,
            IRepository<Medication> medicationRepo,
            IRepository<Test> testRepo)
        {
            _billRepo = billRepo;
            _billItemRepo = billItemRepo;
            _patientRepo = patientRepo;
            _medicalRecordRepo = medicalRecordRepo;
            _appointmentRepo = appointmentRepo;
            _prescriptionRepo = prescriptionRepo;
            _prescriptionItemRepo = prescriptionItemRepo;
            _medicationRepo = medicationRepo;
            _testRepo = testRepo;
        }

        public async Task<int> CreateBillAsync(BillCreateViewModel model, int medicalRecordId)
        {
            var record = await _medicalRecordRepo.GetByIdAsync(medicalRecordId);
            if (record == null)
                throw new Exception($"Không tìm thấy hồ sơ bệnh án với ID = {medicalRecordId}");

            var bill = new Bill
            {
                PatientId = model.PatientId,
                MedicalRecordId = medicalRecordId,
                BillDate = DateTime.Now,
                InsuranceCoverage = model.InsuranceCoverage,
                PaymentStatus = "Unpaid",
                PaymentMethod = "Unknown",
                BillItems = new List<BillItem>()
            };

            foreach (var item in model.Items)
            {
                var subtotal = item.UnitPrice * item.Quantity;
                bill.BillItems.Add(new BillItem
                {
                    ItemName = item.ItemName,
                    ItemType = item.ItemType,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Subtotal = subtotal
                });
            }

            bill.TotalAmount = bill.BillItems.Sum(i => i.Subtotal);
            bill.DueAmount = bill.TotalAmount * (1 - bill.InsuranceCoverage);

            await _billRepo.AddAsync(bill);
            await _billRepo.SaveAsync();

            record.Bill = bill;
            await _medicalRecordRepo.UpdateAsync(record);
            await _medicalRecordRepo.SaveAsync();

            return bill.Id;
        }


        public async Task UpdateBillAsync(BillViewModel model)
        {
            var bill = await _billRepo.GetByIdAsync(model.Id);
            if (bill == null) throw new Exception("Bill not found");

            bill.InsuranceCoverage = model.InsuranceCoverage;
            bill.DueAmount = bill.TotalAmount * (1 - bill.InsuranceCoverage);

            var newPaid = model.PaidAmount;
            var maxAllowable = bill.TotalAmount - bill.PaidAmount;

            if (newPaid > maxAllowable)
                throw new Exception($"Số tiền thanh toán vượt quá còn lại: {maxAllowable}");

            bill.PaidAmount += newPaid;
            bill.DueAmount = bill.TotalAmount - bill.PaidAmount;
            bill.PaymentMethod = model.PaymentMethod;

            bill.PaymentStatus = bill.PaidAmount == 0 ? "Unpaid" :
                                 bill.PaidAmount < bill.TotalAmount ? "Partial" : "Paid";

            await _billRepo.UpdateAsync(bill);
            await _billRepo.SaveAsync();
        }

        public Task<IEnumerable<BillViewModel>> GetAllBillsAsync() => throw new NotImplementedException();

        public async Task<IEnumerable<BillViewModel>> GetBillsByPatientIdAsync(int patientId)
        {
            var bills = await _billRepo.GetAsync(
                filter: b => b.PatientId == patientId,
                includeProperties: "Patient,BillItems,MedicalRecord");

            return bills.Select(b => new BillViewModel
            {
                Id = b.Id,
                PatientId = b.PatientId,
                PatientName = b.Patient.FirstName + " " + b.Patient.LastName,
                MedicalRecordId = b.MedicalRecordId,
                BillDate = b.BillDate,
                TotalAmount = b.TotalAmount,
                PaidAmount = b.PaidAmount,
                DueAmount = b.DueAmount,
                InsuranceCoverage = b.InsuranceCoverage,
                PaymentStatus = b.PaymentStatus,
                PaymentMethod = b.PaymentMethod,
                Items = b.BillItems.Select(i => new BillItemViewModel
                {
                    Id = i.Id,
                    BillId = i.BillId,
                    ItemName = i.ItemName,
                    ItemType = i.ItemType,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal
                }).ToList()
            }).ToList();
        }

        public async Task<BillViewModel> GetBillByIdAsync(int id)
        {
            var bill = await _billRepo.GetFirstOrDefaultAsync(
                filter: b => b.Id == id,
                includeProperties: "Patient,BillItems,MedicalRecord");

            if (bill == null) return null;

            return new BillViewModel
            {
                Id = bill.Id,
                PatientId = bill.PatientId,
                PatientName = bill.Patient?.FirstName + " " + bill.Patient?.LastName,
                MedicalRecordId = bill.MedicalRecordId,
                BillDate = bill.BillDate,
                TotalAmount = bill.TotalAmount,
                PaidAmount = bill.PaidAmount,
                DueAmount = bill.DueAmount,
                InsuranceCoverage = bill.InsuranceCoverage,
                PaymentStatus = bill.PaymentStatus,
                PaymentMethod = bill.PaymentMethod,
                Items = bill.BillItems.Select(bi => new BillItemViewModel
                {
                    Id = bi.Id,
                    BillId = bi.BillId,
                    ItemName = bi.ItemName,
                    ItemType = bi.ItemType,
                    Quantity = bi.Quantity,
                    UnitPrice = bi.UnitPrice,
                    Subtotal = bi.Subtotal
                }).ToList()
            };
        }

        public async Task<BillCreateViewModel> CreateBillFromMedicalRecordAsync(int medicalRecordId)
        {
            // Check: nếu đã có bill nào cho record này thì không cho tạo nữa
            var existingBill = await _billRepo.GetFirstOrDefaultAsync(
                b => b.MedicalRecordId == medicalRecordId);

            if (existingBill != null)
            {
                throw new Exception("Hồ sơ này đã có bill, không thể tạo thêm.");
            }
            var record = await _medicalRecordRepo.GetByIdAsync(medicalRecordId);
            if (record == null) throw new Exception("Không tìm thấy hồ sơ bệnh án.");

            var patient = await _patientRepo.GetByIdAsync(record.PatientId);
            if (patient == null) throw new Exception("Không tìm thấy bệnh nhân.");

            var appointment = await _appointmentRepo.GetByIdAsync(record.AppointmentId);
            if (appointment == null) throw new Exception("Không tìm thấy cuộc hẹn.");

            var doctor = appointment.Doctor ?? await _appointmentRepo.Context().Doctors.FindAsync(appointment.DoctorId);
            if (doctor == null) throw new Exception("Không tìm thấy bác sĩ cho cuộc hẹn.");

            var prescriptions = await _prescriptionRepo.GetAsync(p => p.MedicalRecordId == medicalRecordId);
            var tests = await _testRepo.GetAsync(t => t.MedicalRecordId == medicalRecordId, includeProperties: "Service");

            var items = new List<BillItemCreateViewModel>
            {
                new BillItemCreateViewModel
                {
                    ItemName = "Phí khám",
                    ItemType = "Khám",
                    Quantity = 1,
                    UnitPrice = doctor.ConsultationFee
                }
            };

            foreach (var p in prescriptions)
            {
                var _items = await _prescriptionItemRepo.GetAsync(i => i.PrescriptionId == p.Id);
                if (_items == null) continue;

                foreach (var pi in _items)
                {
                    var med = await _medicationRepo.GetByIdAsync(pi.MedicationId);
                    if (med == null) continue;

                    items.Add(new BillItemCreateViewModel
                    {
                        ItemName = med.Name,
                        ItemType = "Thuốc",
                        Quantity = pi.Quantity,
                        UnitPrice = med.UnitPrice
                    });
                }
            }

            foreach (var test in tests)
            {
                if (test.Service == null)
                {
                    test.Service = await _testRepo.Context().Services.FindAsync(test.ServiceId);
                    if (test.Service == null) continue;
                }

                items.Add(new BillItemCreateViewModel
                {
                    ItemName = test.Service.Name,
                    ItemType = "Dịch vụ",
                    Quantity = 1,
                    UnitPrice = test.Service.Price
                });
            }

            return new BillCreateViewModel
            {
                PatientId = patient.Id,
                PatientName = $"{patient.FirstName} {patient.LastName}",
                HasInsurance = patient.HasInsurance,
                InsuranceProvider = patient.InsuranceProvider,
                InsurancePolicyNumber = patient.InsurancePolicyNumber,
                Items = items,
                MedicalRecordId = medicalRecordId
            };
        }

        public async Task DeleteBillAsync(int id)
        {
            var bill = await _billRepo.GetFirstOrDefaultAsync(
                filter: b => b.Id == id,
                includeProperties: "BillItems");

            if (bill == null) throw new Exception("Bill not found");

            foreach (var item in bill.BillItems.ToList())
            {
                await _billItemRepo.DeleteAsync(item);
            }

            await _billRepo.DeleteAsync(bill);
            await _billRepo.SaveAsync();
        }

        public async Task ApplyPaymentAsync(int billId, decimal newPaidAmount, string paymentMethod)
        {
            var bill = await _billRepo.GetByIdAsync(billId);
            if (bill == null) throw new Exception("Bill not found");

            if (newPaidAmount <= 0) throw new Exception("Invalid payment amount");

            var discountedTotal = Math.Round(bill.TotalAmount * (1 - bill.InsuranceCoverage), 2);
            var remaining = discountedTotal - bill.PaidAmount;

            if (newPaidAmount > remaining)
                throw new Exception($"Số tiền vượt quá số còn lại: {remaining}");

            bill.PaidAmount += newPaidAmount;
            bill.DueAmount = Math.Max(0, discountedTotal - bill.PaidAmount);
            bill.PaymentMethod = paymentMethod;

            bill.PaymentStatus = bill.PaidAmount == 0 ? "Unpaid"
                   : bill.DueAmount > 0 ? "Partial"
                   : "Paid";
            

            await _billRepo.UpdateAsync(bill);
            await _billRepo.SaveAsync();
        }

        public Task<decimal> CalculateTotalRevenueAsync() => throw new NotImplementedException();
        public Task<decimal> CalculateMonthlyRevenueAsync() => throw new NotImplementedException();

        public Task<int> CreateBillAsync(BillCreateViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<BillViewModel> GetBillByMedicalRecordIdAsync(int medicalRecordId)
        {
            var bill = await _billRepo.GetFirstOrDefaultAsync(
                filter: b => b.MedicalRecordId == medicalRecordId,
                includeProperties: "Patient,BillItems,MedicalRecord"
            );

            if (bill == null) return null;

            return new BillViewModel
            {
                Id = bill.Id,
                PatientId = bill.PatientId,
                PatientName = bill.Patient?.FirstName + " " + bill.Patient?.LastName,
                MedicalRecordId = medicalRecordId,
                BillDate = bill.BillDate,
                TotalAmount = bill.TotalAmount,
                PaidAmount = bill.PaidAmount,
                DueAmount = bill.DueAmount,
                InsuranceCoverage = bill.InsuranceCoverage,
                PaymentStatus = bill.PaymentStatus,
                PaymentMethod = bill.PaymentMethod,
                Items = bill.BillItems.Select(i => new BillItemViewModel
                {
                    Id = i.Id,
                    BillId = i.BillId,
                    ItemName = i.ItemName,
                    ItemType = i.ItemType,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal
                }).ToList()
            };
        }

    }
}
