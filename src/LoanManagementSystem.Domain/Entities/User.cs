using System;
using System.Collections.Generic;

namespace LoanManagementSystem.Domain.Entities
{
    /// <summary>
    /// Represents a user in the system
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties
        public virtual Role Role { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }

    /// <summary>
    /// Represents a role in the system
    /// </summary>
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

    /// <summary>
    /// Represents a permission in the system
    /// </summary>
    public class Permission
    {
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

    /// <summary>
    /// Represents role-permission mapping
    /// </summary>
    public class RolePermission
    {
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        // Navigation Properties
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }

    /// <summary>
    /// Represents a customer in the system
    /// </summary>
    public class Customer
    {
        public int CustomerId { get; set; }
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
        public int? CreditScore { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Navigation Properties
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }

    /// <summary>
    /// Represents a loan in the system
    /// </summary>
    public class Loan
    {
        public int LoanId { get; set; }
        public string LoanNumber { get; set; }
        public int CustomerId { get; set; }
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
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        // Navigation Properties
        public virtual Customer Customer { get; set; }
        public virtual ICollection<Installment> Installments { get; set; } = new List<Installment>();
        public virtual ICollection<Guarantor> Guarantors { get; set; } = new List<Guarantor>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<TreasuryTransaction> TreasuryTransactions { get; set; } = new List<TreasuryTransaction>();
    }

    /// <summary>
    /// Represents an installment for a loan
    /// </summary>
    public class Installment
    {
        public int InstallmentId { get; set; }
        public int LoanId { get; set; }
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal AmountDue { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string Status { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal LateCharges { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public decimal RemainingBalance => AmountDue - AmountPaid;
        public int DaysOverdue => DateTime.Now > DueDate ? (DateTime.Now - DueDate).Days : 0;

        // Navigation Properties
        public virtual Loan Loan { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<LatePaymentPenalty> LatePaymentPenalties { get; set; } = new List<LatePaymentPenalty>();
    }

    /// <summary>
    /// Represents a payment for an installment
    /// </summary>
    public class Payment
    {
        public int PaymentId { get; set; }
        public int InstallmentId { get; set; }
        public int LoanId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual Installment Installment { get; set; }
        public virtual Loan Loan { get; set; }
        public virtual TreasuryTransaction TreasuryTransaction { get; set; }
    }

    /// <summary>
    /// Represents a guarantor for a loan
    /// </summary>
    public class Guarantor
    {
        public int GuarantorId { get; set; }
        public int LoanId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Relationship { get; set; }
        public decimal GuaranteeAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Navigation Properties
        public virtual Loan Loan { get; set; }
    }

    /// <summary>
    /// Represents a treasury transaction
    /// </summary>
    public class TreasuryTransaction
    {
        public int TreasuryId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int? RelatedLoanId { get; set; }
        public int? RelatedPaymentId { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual Loan RelatedLoan { get; set; }
        public virtual Payment RelatedPayment { get; set; }
    }

    /// <summary>
    /// Represents a late payment penalty
    /// </summary>
    public class LatePaymentPenalty
    {
        public int PenaltyId { get; set; }
        public int InstallmentId { get; set; }
        public decimal PenaltyPercentage { get; set; }
        public decimal PenaltyAmount { get; set; }
        public int DaysOverdue { get; set; }
        public DateTime AppliedDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual Installment Installment { get; set; }
    }

    /// <summary>
    /// Represents an audit log entry
    /// </summary>
    public class AuditLog
    {
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string IPAddress { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
    }
}