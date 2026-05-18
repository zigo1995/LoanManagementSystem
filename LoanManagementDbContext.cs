using Microsoft.EntityFrameworkCore;
using LoanManagementSystem.Domain.Entities;

namespace LoanManagementSystem.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for Loan Management System
    /// </summary>
    public class LoanManagementDbContext : DbContext
    {
        public LoanManagementDbContext(DbContextOptions<LoanManagementDbContext> options)
            : base(options)
        {
        }

        #region DbSets

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Installment> Installments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Guarantor> Guarantors { get; set; }
        public DbSet<TreasuryTransaction> TreasuryTransactions { get; set; }
        public DbSet<LatePaymentPenalty> LatePaymentPenalties { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.HasIndex(e => e.NationalId).IsUnique();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
                entity.HasMany(e => e.Loans)
                    .WithOne(l => l.Customer)
                    .HasForeignKey(l => l.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Loan
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(e => e.LoanId);
                entity.HasIndex(e => e.LoanNumber).IsUnique();
                entity.Property(e => e.LoanAmount).HasPrecision(12, 2);
                entity.Property(e => e.InterestRate).HasPrecision(5, 2);
                entity.Property(e => e.BalanceAmount).HasPrecision(12, 2);
                entity.Property(e => e.TotalInterest).HasPrecision(12, 2);
                entity.Property(e => e.TotalPaid).HasPrecision(12, 2);
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Loans)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Installment
            modelBuilder.Entity<Installment>(entity =>
            {
                entity.HasKey(e => e.InstallmentId);
                entity.HasIndex(e => new { e.LoanId, e.InstallmentNumber }).IsUnique();
                entity.Property(e => e.AmountDue).HasPrecision(12, 2);
                entity.Property(e => e.PrincipalAmount).HasPrecision(12, 2);
                entity.Property(e => e.InterestAmount).HasPrecision(12, 2);
                entity.Property(e => e.AmountPaid).HasPrecision(12, 2);
                entity.Property(e => e.LateCharges).HasPrecision(12, 2);
                entity.HasOne(e => e.Loan)
                    .WithMany(l => l.Installments)
                    .HasForeignKey(e => e.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasPrecision(12, 2);
                entity.HasOne(e => e.Installment)
                    .WithMany(i => i.Payments)
                    .HasForeignKey(e => e.InstallmentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Loan)
                    .WithMany(l => l.Payments)
                    .HasForeignKey(e => e.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Guarantor
            modelBuilder.Entity<Guarantor>(entity =>
            {
                entity.HasKey(e => e.GuarantorId);
                entity.Property(e => e.GuaranteeAmount).HasPrecision(12, 2);
                entity.HasOne(e => e.Loan)
                    .WithMany(l => l.Guarantors)
                    .HasForeignKey(e => e.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TreasuryTransaction
            modelBuilder.Entity<TreasuryTransaction>(entity =>
            {
                entity.HasKey(e => e.TreasuryId);
                entity.Property(e => e.Amount).HasPrecision(12, 2);
                entity.HasOne(e => e.RelatedLoan)
                    .WithMany(l => l.TreasuryTransactions)
                    .HasForeignKey(e => e.RelatedLoanId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.RelatedPayment)
                    .WithOne(p => p.TreasuryTransaction)
                    .HasForeignKey<TreasuryTransaction>(e => e.RelatedPaymentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure LatePaymentPenalty
            modelBuilder.Entity<LatePaymentPenalty>(entity =>
            {
                entity.HasKey(e => e.PenaltyId);
                entity.Property(e => e.PenaltyPercentage).HasPrecision(5, 2);
                entity.Property(e => e.PenaltyAmount).HasPrecision(12, 2);
                entity.HasOne(e => e.Installment)
                    .WithMany(i => i.LatePaymentPenalties)
                    .HasForeignKey(e => e.InstallmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.AuditId);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.AuditLogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            // Configure Permission
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.PermissionId);
                entity.HasIndex(e => e.PermissionCode).IsUnique();
            });

            // Configure RolePermission
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.RolePermissionId);
                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}