using TaskManagement.DataAccessLayer.Entities;

namespace TaskManagement.Business.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
