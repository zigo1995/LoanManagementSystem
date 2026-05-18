using System;

namespace LoanManagementSystem.Application.DTOs
{
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

    public class PaymentStatisticsDto
    {
        public int TotalPayments { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TodayAmount { get; set; }
        public decimal ThisMonthAmount { get; set; }
        public decimal ThisYearAmount { get; set; }
        public decimal AveragePayment { get; set; }
    }
}