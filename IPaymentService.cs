using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoanManagementSystem.Application.DTOs;

namespace LoanManagementSystem.Application.Services
{
    /// <summary>
    /// Payment Service Interface
    /// </summary>
    public interface IPaymentService
    {
        Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto dto, int userId);
        Task<PaymentDto> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByLoanAsync(int loanId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByInstallmentAsync(int installmentId);
        Task<bool> UpdatePaymentAsync(int paymentId, UpdatePaymentDto dto, int userId);
        Task<PaymentStatisticsDto> GetPaymentStatisticsAsync();
        Task<decimal> CalculateTotalDueAsync(int installmentId);
    }
}