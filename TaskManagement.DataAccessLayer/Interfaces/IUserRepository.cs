
using TaskManagement.DataAccessLayer.Entities;

namespace TaskManagement.DataAccessLayer.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<bool> UserExistsAsync(string email);
        Task<User> GetUserWithTasksAsync(int userId);

    }
}