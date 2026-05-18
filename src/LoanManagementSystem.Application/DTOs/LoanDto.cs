using System;

namespace LoanManagementSystem.Application.DTOs
{
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
}