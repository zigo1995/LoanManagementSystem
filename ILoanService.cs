using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoanManagementSystem.Application.DTOs;

namespace LoanManagementSystem.Application.Services
{
    /// <summary>
    /// Loan Service Interface
    /// </summary>
    public interface ILoanService
    {
        Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, int userId);
        Task<LoanDto> GetLoanByIdAsync(int loanId);
        Task<IEnumerable<LoanDto>> GetLoansByCustomerAsync(int customerId);
        Task<IEnumerable<LoanDto>> GetAllLoansAsync();
        Task<bool> UpdateLoanAsync(int loanId, UpdateLoanDto dto, int userId);
        Task<bool> DeleteLoanAsync(int loanId, int userId);
        Task<decimal> CalculateLoanInterestAsync(decimal principal, decimal rate, int months);
        Task<List<InstallmentCalculationDto>> GenerateInstallmentScheduleAsync(int loanId);
        Task<LoanStatisticsDto> GetLoanStatisticsAsync();
    }
}