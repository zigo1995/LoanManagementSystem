using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LoanManagementSystem.Domain.Entities;
using LoanManagementSystem.Infrastructure.Data;
using LoanManagementSystem.Application.DTOs;

namespace LoanManagementSystem.Application.Services
{
    /// <summary>
    /// Payment Service Implementation
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly LoanManagementDbContext _context;
        private readonly IAuditService _auditService;

        public PaymentService(LoanManagementDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto dto, int userId)
        {
            try
            {
                var installment = await _context.Installments
                    .Include(i => i.Loan)
                    .FirstOrDefaultAsync(i => i.InstallmentId == dto.InstallmentId);

                if (installment == null)
                    throw new Exception("القسط غير موجود"); // Installment not found

                if (dto.Amount <= 0)
                    throw new Exception("المبلغ يجب أن يكون أكبر من صفر");

                if (dto.Amount > installment.AmountDue - installment.AmountPaid)
                    throw new Exception("المبلغ المدفوع يتجاوز المبلغ المستحق");

                // Apply late charges if overdue
                decimal lateCharges = 0;
                if (DateTime.Now > installment.DueDate && installment.Status == "Overdue")
                {
                    lateCharges = await CalculateLateChargesAsync(installment);
                }

                var payment = new Payment
                {
                    InstallmentId = dto.InstallmentId,
                    LoanId = installment.LoanId,
                    PaymentDate = DateTime.Now,
                    Amount = dto.Amount,
                    PaymentMethod = dto.PaymentMethod,
                    ReferenceNumber = dto.ReferenceNumber ?? GenerateReferenceNumber(),
                    Notes = dto.Notes,
                    ProcessedBy = (await _context.Users.FindAsync(userId))?.Username ?? "System"
                };

                await _context.Payments.AddAsync(payment);

                // Update installment
                installment.AmountPaid += dto.Amount;
                installment.LateCharges = lateCharges;
                installment.UpdatedAt = DateTime.Now;

                // Update installment status
                if (installment.AmountPaid >= installment.AmountDue)
                {
                    installment.Status = "Paid";
                    installment.PaidDate = DateTime.Now;
                }
                else if (installment.AmountPaid > 0)
                {
                    installment.Status = "Partial";
                }

                _context.Installments.Update(installment);

                // Update loan total paid
                var loan = installment.Loan;
                loan.TotalPaid += dto.Amount;

                // Check if loan is completed
                var totalDue = await _context.Installments
                    .Where(i => i.LoanId == loan.LoanId)
                    .SumAsync(i => i.AmountDue);

                if (loan.TotalPaid >= totalDue)
                {
                    loan.Status = "Completed";
                    loan.BalanceAmount = 0;
                }
                else
                {
                    loan.BalanceAmount = totalDue - loan.TotalPaid;
                }

                _context.Loans.Update(loan);

                // Create treasury transaction
                var treasury = new TreasuryTransaction
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Credit",
                    Amount = dto.Amount,
                    Description = $"دفع القسط {installment.InstallmentNumber} - {loan.LoanNumber}",
                    RelatedLoanId = loan.LoanId,
                    RelatedPaymentId = payment.PaymentId,
                    ProcessedBy = (await _context.Users.FindAsync(userId))?.Username ?? "System"
                };

                await _context.TreasuryTransactions.AddAsync(treasury);
                await _context.SaveChangesAsync();

                // Log audit
                await _auditService.LogAsync(userId, "CREATE", "Payments", payment.PaymentId, null, payment.PaymentId.ToString());

                return MapToDto(payment);
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في معالجة الدفع: {ex.Message}");
            }
        }

        public async Task<PaymentDto> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Installment)
                .Include(p => p.Loan)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                throw new Exception("الدفع غير موجود");

            return MapToDto(payment);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByLoanAsync(int loanId)
        {
            var payments = await _context.Payments
                .Where(p => p.LoanId == loanId)
                .Include(p => p.Installment)
                .ToListAsync();

            return payments.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByInstallmentAsync(int installmentId)
        {
            var payments = await _context.Payments
                .Where(p => p.InstallmentId == installmentId)
                .ToListAsync();

            return payments.Select(MapToDto).ToList();
        }

        public async Task<bool> UpdatePaymentAsync(int paymentId, UpdatePaymentDto dto, int userId)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(paymentId);
                if (payment == null)
                    throw new Exception("الدفع غير موجود");

                payment.Notes = dto.Notes ?? payment.Notes;
                payment.CreatedAt = DateTime.Now;

                _context.Payments.Update(payment);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في تحديث الدفع: {ex.Message}");
            }
        }

        public async Task<PaymentStatisticsDto> GetPaymentStatisticsAsync()
        {
            var payments = await _context.Payments.ToListAsync();
            var today = DateTime.Now.Date;
            var thisMonth = payments.Where(p => p.PaymentDate.Month == today.Month && p.PaymentDate.Year == today.Year).ToList();
            var thisYear = payments.Where(p => p.PaymentDate.Year == today.Year).ToList();

            return new PaymentStatisticsDto
            {
                TotalPayments = payments.Count,
                TotalAmount = payments.Sum(p => p.Amount),
                TodayAmount = payments.Where(p => p.PaymentDate.Date == today).Sum(p => p.Amount),
                ThisMonthAmount = thisMonth.Sum(p => p.Amount),
                ThisYearAmount = thisYear.Sum(p => p.Amount),
                AveragePayment = payments.Any() ? payments.Average(p => p.Amount) : 0
            };
        }

        public async Task<decimal> CalculateTotalDueAsync(int installmentId)
        {
            var installment = await _context.Installments.FindAsync(installmentId);
            if (installment == null)
                throw new Exception("القسط غير موجود");

            var lateCharges = await CalculateLateChargesAsync(installment);
            return (installment.AmountDue - installment.AmountPaid) + lateCharges;
        }

        #region Helper Methods

        private async Task<decimal> CalculateLateChargesAsync(Installment installment)
        {
            if (DateTime.Now <= installment.DueDate)
                return 0;

            var daysOverdue = (DateTime.Now - installment.DueDate).Days;
            var penaltyPercentage = 2m; // 2% per month or 0.067% per day

            var penalty = (installment.AmountDue * penaltyPercentage / 100) * (daysOverdue / 30m);

            // Cap penalty at 10% of installment amount
            var maxPenalty = installment.AmountDue * 0.1m;
            return Math.Min(penalty, maxPenalty);
        }

        private string GenerateReferenceNumber()
        {
            return $"PAY{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                InstallmentId = payment.InstallmentId,
                InstallmentNumber = payment.Installment?.InstallmentNumber,
                LoanId = payment.LoanId,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                ReferenceNumber = payment.ReferenceNumber,
                Notes = payment.Notes,
                ProcessedBy = payment.ProcessedBy
            };
        }

        #endregion
    }
}