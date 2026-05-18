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
    /// Loan Service Implementation
    /// </summary>
    public class LoanService : ILoanService
    {
        private readonly LoanManagementDbContext _context;
        private readonly IAuditService _auditService;

        public LoanService(LoanManagementDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, int userId)
        {
            try
            {
                // Validate customer exists
                var customer = await _context.Customers.FindAsync(dto.CustomerId);
                if (customer == null)
                    throw new Exception("العميل غير موجود"); // Customer not found

                // Generate unique loan number
                var loanNumber = await GenerateLoanNumberAsync();

                // Calculate total interest
                var totalInterest = CalculateLoanInterest(dto.LoanAmount, dto.InterestRate, dto.LoanTermMonths);
                var endDate = DateTime.Now.AddMonths(dto.LoanTermMonths);

                var loan = new Loan
                {
                    LoanNumber = loanNumber,
                    CustomerId = dto.CustomerId,
                    LoanAmount = dto.LoanAmount,
                    InterestRate = dto.InterestRate,
                    LoanTermMonths = dto.LoanTermMonths,
                    StartDate = DateTime.Now,
                    EndDate = endDate,
                    Status = "Active",
                    LoanType = dto.LoanType,
                    Purpose = dto.Purpose,
                    BalanceAmount = dto.LoanAmount + totalInterest,
                    TotalInterest = totalInterest,
                    TotalPaid = 0,
                    CreatedBy = (await _context.Users.FindAsync(userId))?.Username ?? "System"
                };

                await _context.Loans.AddAsync(loan);
                await _context.SaveChangesAsync();

                // Generate installments
                await GenerateInstallmentsAsync(loan.LoanId, loan.LoanAmount, totalInterest, dto.LoanTermMonths);

                // Log audit
                await _auditService.LogAsync(userId, "CREATE", "Loans", loan.LoanId, null, loan.LoanNumber);

                // Create treasury transaction
                var treasury = new TreasuryTransaction
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Debit",
                    Amount = dto.LoanAmount,
                    Description = $"قرض جديد - {loanNumber}", // New loan
                    RelatedLoanId = loan.LoanId,
                    ProcessedBy = (await _context.Users.FindAsync(userId))?.Username ?? "System"
                };

                await _context.TreasuryTransactions.AddAsync(treasury);
                await _context.SaveChangesAsync();

                return MapToDto(loan);
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في إنشاء القرض: {ex.Message}");
            }
        }

        public async Task<LoanDto> GetLoanByIdAsync(int loanId)
        {
            var loan = await _context.Loans
                .Include(l => l.Customer)
                .Include(l => l.Installments)
                .Include(l => l.Guarantors)
                .FirstOrDefaultAsync(l => l.LoanId == loanId && !l.IsDeleted);

            if (loan == null)
                throw new Exception("القرض غير موجود"); // Loan not found

            return MapToDto(loan);
        }

        public async Task<IEnumerable<LoanDto>> GetLoansByCustomerAsync(int customerId)
        {
            var loans = await _context.Loans
                .Where(l => l.CustomerId == customerId && !l.IsDeleted)
                .Include(l => l.Installments)
                .ToListAsync();

            return loans.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
        {
            var loans = await _context.Loans
                .Where(l => !l.IsDeleted)
                .Include(l => l.Customer)
                .Include(l => l.Installments)
                .ToListAsync();

            return loans.Select(MapToDto).ToList();
        }

        public async Task<bool> UpdateLoanAsync(int loanId, UpdateLoanDto dto, int userId)
        {
            try
            {
                var loan = await _context.Loans.FindAsync(loanId);
                if (loan == null)
                    throw new Exception("القرض غير موجود");

                var oldValues = MapToDto(loan);

                loan.Status = dto.Status ?? loan.Status;
                loan.Purpose = dto.Purpose ?? loan.Purpose;
                loan.UpdatedAt = DateTime.Now;
                loan.UpdatedBy = (await _context.Users.FindAsync(userId))?.Username ?? "System";

                _context.Loans.Update(loan);
                var result = await _context.SaveChangesAsync() > 0;

                if (result)
                {
                    var newValues = MapToDto(loan);
                    await _auditService.LogAsync(userId, "UPDATE", "Loans", loanId, 
                        System.Text.Json.JsonSerializer.Serialize(oldValues),
                        System.Text.Json.JsonSerializer.Serialize(newValues));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في تحديث القرض: {ex.Message}");
            }
        }

        public async Task<bool> DeleteLoanAsync(int loanId, int userId)
        {
            try
            {
                var loan = await _context.Loans.FindAsync(loanId);
                if (loan == null)
                    throw new Exception("القرض غير موجود");

                loan.IsDeleted = true;
                loan.UpdatedAt = DateTime.Now;
                loan.UpdatedBy = (await _context.Users.FindAsync(userId))?.Username ?? "System";

                _context.Loans.Update(loan);
                var result = await _context.SaveChangesAsync() > 0;

                if (result)
                {
                    await _auditService.LogAsync(userId, "DELETE", "Loans", loanId, 
                        System.Text.Json.JsonSerializer.Serialize(MapToDto(loan)), null);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في حذف القرض: {ex.Message}");
            }
        }

        public Task<decimal> CalculateLoanInterestAsync(decimal principal, decimal rate, int months)
        {
            var interest = CalculateLoanInterest(principal, rate, months);
            return Task.FromResult(interest);
        }

        public async Task<List<InstallmentCalculationDto>> GenerateInstallmentScheduleAsync(int loanId)
        {
            var loan = await _context.Loans
                .Include(l => l.Installments)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);

            if (loan == null)
                throw new Exception("القرض غير موجود");

            return loan.Installments
                .OrderBy(i => i.InstallmentNumber)
                .Select(i => new InstallmentCalculationDto
                {
                    InstallmentNumber = i.InstallmentNumber,
                    DueDate = i.DueDate,
                    PrincipalAmount = i.PrincipalAmount,
                    InterestAmount = i.InterestAmount,
                    AmountDue = i.AmountDue,
                    AmountPaid = i.AmountPaid,
                    Status = i.Status
                })
                .ToList();
        }

        public async Task<LoanStatisticsDto> GetLoanStatisticsAsync()
        {
            var loans = await _context.Loans.Where(l => !l.IsDeleted).ToListAsync();
            var installments = await _context.Installments.ToListAsync();
            var payments = await _context.Payments.ToListAsync();

            return new LoanStatisticsDto
            {
                TotalLoans = loans.Count,
                ActiveLoans = loans.Count(l => l.Status == "Active"),
                CompletedLoans = loans.Count(l => l.Status == "Completed"),
                DefaultedLoans = loans.Count(l => l.Status == "Defaulted"),
                TotalLoanAmount = loans.Sum(l => l.LoanAmount),
                TotalPaidAmount = loans.Sum(l => l.TotalPaid),
                TotalBalanceAmount = loans.Sum(l => l.BalanceAmount),
                TotalInterest = loans.Sum(l => l.TotalInterest),
                OverdueInstallments = installments.Count(i => i.Status == "Overdue"),
                PendingInstallments = installments.Count(i => i.Status == "Pending"),
                TotalInstallmentsPaid = payments.Sum(p => p.Amount)
            };
        }

        #region Helper Methods

        private async Task<string> GenerateLoanNumberAsync()
        {
            var lastLoan = await _context.Loans
                .OrderByDescending(l => l.LoanId)
                .FirstOrDefaultAsync();

            var number = lastLoan == null ? 1 : int.Parse(lastLoan.LoanNumber.Substring(4)) + 1;
            return $"LOAN{number:D6}";
        }

        private decimal CalculateLoanInterest(decimal principal, decimal rate, int months)
        {
            // Simple interest calculation: (Principal * Rate * Time) / 100
            return (principal * (rate / 100) * months) / 12;
        }

        private async Task GenerateInstallmentsAsync(int loanId, decimal principal, decimal totalInterest, int months)
        {
            var principalPerInstallment = principal / months;
            var interestPerInstallment = totalInterest / months;
            var amountPerInstallment = principalPerInstallment + interestPerInstallment;

            for (int i = 1; i <= months; i++)
            {
                var dueDate = DateTime.Now.AddMonths(i);

                var installment = new Installment
                {
                    LoanId = loanId,
                    InstallmentNumber = i,
                    DueDate = dueDate,
                    PrincipalAmount = principalPerInstallment,
                    InterestAmount = interestPerInstallment,
                    AmountDue = amountPerInstallment,
                    AmountPaid = 0,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                await _context.Installments.AddAsync(installment);
            }

            await _context.SaveChangesAsync();
        }

        private LoanDto MapToDto(Loan loan)
        {
            return new LoanDto
            {
                LoanId = loan.LoanId,
                LoanNumber = loan.LoanNumber,
                CustomerId = loan.CustomerId,
                CustomerName = loan.Customer?.FullName,
                LoanAmount = loan.LoanAmount,
                InterestRate = loan.InterestRate,
                LoanTermMonths = loan.LoanTermMonths,
                StartDate = loan.StartDate,
                EndDate = loan.EndDate,
                Status = loan.Status,
                LoanType = loan.LoanType,
                Purpose = loan.Purpose,
                BalanceAmount = loan.BalanceAmount,
                TotalInterest = loan.TotalInterest,
                TotalPaid = loan.TotalPaid,
                CreatedAt = loan.CreatedAt,
                CreatedBy = loan.CreatedBy
            };
        }

        #endregion
    }
}