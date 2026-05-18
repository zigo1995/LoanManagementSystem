using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LoanManagementSystem.Domain.Entities;
using LoanManagementSystem.Infrastructure.Data;

namespace LoanManagementSystem.Application.Services
{
    /// <summary>
    /// Audit Service Implementation
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly LoanManagementDbContext _context;

        public AuditService(LoanManagementDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int userId, string action, string tableName, int recordId, string oldValues, string newValues)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    TableName = tableName,
                    RecordId = recordId,
                    OldValues = oldValues,
                    NewValues = newValues,
                    IPAddress = GetIPAddress(),
                    CreatedAt = DateTime.Now
                };

                await _context.AuditLogs.AddAsync(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't throw - audit failures shouldn't break the application
                System.Diagnostics.Debug.WriteLine($"Audit log error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(DateTime from, DateTime to)
        {
            return await _context.AuditLogs
                .Where(l => l.CreatedAt >= from && l.CreatedAt <= to)
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(int userId)
        {
            return await _context.AuditLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        private string GetIPAddress()
        {
            try
            {
                var hostName = System.Net.Dns.GetHostName();
                var addresses = System.Net.Dns.GetHostAddresses(hostName);
                return addresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}