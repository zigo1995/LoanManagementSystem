using System;
using System.Collections.Generic;

namespace LoanManagementSystem.Application.DTOs
{
    // Customer DTOs
    public class CreateCustomerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Occupation { get; set; }
        public decimal? MonthlyIncome { get; set; }
    }

    public class UpdateCustomerDto
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Occupation { get; set; }
        public decimal? MonthlyIncome { get; set; }
    }

    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Occupation { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public int? CreditScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }

    // Loan DTOs
    public class CreateLoanDto
    {
        public int CustomerId { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int LoanTermMonths { get; set; }
        public string LoanType { get; set; }
        public string Purpose { get; set; }
    }

    public class UpdateLoanDto
    {
        public string Status { get; set; }
        public string Purpose { get; set; }
    }

    public class LoanDto
    {
        public int LoanId { get; set; }
        public string LoanNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int LoanTermMonths { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string LoanType { get; set; }
        public string Purpose { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }

    // Payment DTOs
    public class ProcessPaymentDto
    {
        public int InstallmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
    }

    public class UpdatePaymentDto
    {
        public string Notes { get; set; }
    }

    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int InstallmentId { get; set; }
        public int? InstallmentNumber { get; set; }
        public int LoanId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
    }

    // Installment DTOs
    public class InstallmentCalculationDto
    {
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public string Status { get; set; }
    }

    // Statistics DTOs
    public class LoanStatisticsDto
    {
        public int TotalLoans { get; set; }
        public int ActiveLoans { get; set; }
        public int CompletedLoans { get; set; }
        public int DefaultedLoans { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalBalanceAmount { get; set; }
        public decimal TotalInterest { get; set; }
        public int OverdueInstallments { get; set; }
        public int PendingInstallments { get; set; }
        public decimal TotalInstallmentsPaid { get; set; }
    }

    public class PaymentStatisticsDto
    {
        public int TotalPayments { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TodayAmount { get; set; }
        public decimal ThisMonthAmount { get; set; }
        public decimal ThisYearAmount { get; set; }
        public decimal AveragePayment { get; set; }
    }

    // Guarantor DTOs
    public class CreateGuarantorDto
    {
        public int LoanId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Relationship { get; set; }
        public decimal GuaranteeAmount { get; set; }
    }

    public class GuarantorDto
    {
        public int GuarantorId { get; set; }
        public int LoanId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Relationship { get; set; }
        public decimal GuaranteeAmount { get; set; }
        public bool IsActive { get; set; }
    }
}