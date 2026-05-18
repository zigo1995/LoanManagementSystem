using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LoanManagementSystem.Infrastructure.Data;
using LoanManagementSystem.Application.Services;
using LoanManagementSystem.Infrastructure.Repositories;

namespace LoanManagementSystem.Presentation
{
    /// <summary>
    /// Service Configuration for Dependency Injection
    /// </summary>
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
        {
            // Database Context
            services.AddDbContext<LoanManagementDbContext>(options =>
                options.UseSqlServer(connectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelaySeconds: 5,
                        errorNumbersToAdd: null)));

            // Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IGuarantorService, GuarantorService>();
            services.AddScoped<ITreasuryService, TreasuryService>();
            services.AddScoped<IReportService, ReportService>();

            return services;
        }
    }
}