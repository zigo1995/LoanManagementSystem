using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LoanManagementSystem.Domain.Entities;
using LoanManagementSystem.Infrastructure.Data;
using LoanManagementSystem.Infrastructure.Repositories;

namespace LoanManagementSystem.Application.Services
{
    /// <summary>
    /// Authentication Service Implementation
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly LoanManagementDbContext _context;

        public AuthenticationService(LoanManagementDbContext context)
        {
            _context = context;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
        {
            var result = new AuthenticationResult();

            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);

                if (user == null)
                {
                    result.IsSuccessful = false;
                    result.Message = "اسم المستخدم غير صحيح"; // Username incorrect
                    return result;
                }

                if (!user.IsActive)
                {
                    result.IsSuccessful = false;
                    result.Message = "حساب المستخدم غير مفعل"; // User account is not active
                    return result;
                }

                if (!VerifyPassword(password, user.PasswordHash))
                {
                    result.IsSuccessful = false;
                    result.Message = "كلمة المرور غير صحيحة"; // Password incorrect
                    return result;
                }

                // Update last login
                user.LastLogin = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                result.IsSuccessful = true;
                result.Message = "تم تسجيل الدخول بنجاح"; // Login successful
                result.UserId = user.UserId;
                result.Username = user.Username;
                result.FullName = user.FullName;
                result.RoleId = user.RoleId;
                result.RoleName = user.Role?.RoleName;

                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Message = $"خطأ في المصادقة: {ex.Message}";
                return result;
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive && !u.IsDeleted);

            if (user == null)
                return false;

            return VerifyPassword(password, user.PasswordHash);
        }

        public string HashPassword(string password)
        {
            // Using BCrypt for password hashing
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}