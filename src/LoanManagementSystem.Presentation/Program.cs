using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using LoanManagementSystem.Infrastructure.Data;
using LoanManagementSystem.Application.Services;
using LoanManagementSystem.Infrastructure.Repositories;
using LoanManagementSystem.Presentation.Forms;

namespace LoanManagementSystem.Presentation
{
    static class Program
    {
        public static IServiceProvider ServiceProvider { get; set; }

        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.DefaultFont = new System.Drawing.Font("Segoe UI", 10F);
                Application.SetHighDpiMode(HighDpiMode.SystemAware);

                var services = new ServiceCollection();

                // Database connection
                string connectionString = "Server=.;Database=LoanManagementDB;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;";

                services.AddDbContext<LoanManagementDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Register services
                services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                services.AddScoped<IAuthenticationService, AuthenticationService>();
                services.AddScoped<IAuditService, AuditService>();
                services.AddScoped<ILoanService, LoanService>();
                services.AddScoped<IPaymentService, PaymentService>();

                ServiceProvider = services.BuildServiceProvider();

                Application.Run(new LoginForm(ServiceProvider.GetRequiredService<IAuthenticationService>()));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في بدء التطبيق:\n\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
    }
}