using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using LoanManagementSystem.Application.Services;
using LoanManagementSystem.Application.DTOs;
using LoanManagementSystem.Infrastructure.Data;

namespace LoanManagementSystem.Presentation.Forms
{
    /// <summary>
    /// Main Dashboard Form
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly ILoanService _loanService;
        private readonly IPaymentService _paymentService;
        private LoanManagementDbContext _context;

        public MainForm()
        {
            InitializeComponent();
            _context = new LoanManagementDbContext(
                new Microsoft.EntityFrameworkCore.DbContextOptions<LoanManagementDbContext>()
            );
            ConfigureArabicUI();
            LoadDashboardData();
        }

        private void ConfigureArabicUI()
        {
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Text = "نظام إدارة القروض - لوحة التحكم"; // Loan Management System - Dashboard
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
        }

        private async void LoadDashboardData()
        {
            try
            {
                // Create services
                var auditService = new AuditService(_context);
                _loanService = new LoanService(_context, auditService);
                _paymentService = new PaymentService(_context, auditService);

                // Load statistics
                var loanStats = await _loanService.GetLoanStatisticsAsync();
                var paymentStats = await _paymentService.GetPaymentStatisticsAsync();

                DisplayStatistics(loanStats, paymentStats);
                LoadRecentTransactions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayStatistics(LoanStatisticsDto loanStats, PaymentStatisticsDto paymentStats)
        {
            // This would display the statistics in UI controls
            // Implementation depends on specific UI requirements
        }

        private async void LoadRecentTransactions()
        {
            try
            {
                var loans = await _loanService.GetAllLoansAsync();
                // Bind to DataGridView or other control
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            // Implementation of UI components
        }
    }
}