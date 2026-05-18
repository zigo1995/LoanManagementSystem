using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoanManagementSystem.Domain.Entities;

namespace LoanManagementSystem.Application.Services
{
    /// <summary>
    /// Audit Service Interface
    /// </summary>
    public interface IAuditService
    {
        Task LogAsync(int userId, string action, string tableName, int recordId, string oldValues, string newValues);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(DateTime from, DateTime to);
        Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(int userId);
    }
}