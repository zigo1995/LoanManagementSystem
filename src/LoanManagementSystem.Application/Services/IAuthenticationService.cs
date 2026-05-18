using System.Threading.Tasks;

namespace LoanManagementSystem.Application.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string username, string password);
        Task<bool> ValidateCredentialsAsync(string username, string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class AuthenticationResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
    }
}