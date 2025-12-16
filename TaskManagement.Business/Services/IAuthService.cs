
using TaskManagement.DataAccessLayer.Entities;

namespace TaskManagement.Business.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string email, string password);
        Task<User> RegisterAsync(RegisterDto registerDto);
        string GenerateJwtToken(User user);
    }
}
