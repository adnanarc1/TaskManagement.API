// TaskManagement.DataAccessLayer/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccessLayer.Entities;
using TaskManagement.DataAccessLayer.Interfaces;
using TaskManagement.DataAccessLayer.Data;

namespace TaskManagement.DataAccessLayer.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => u.Role == role && u.IsActive)
                .ToListAsync();
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<User> GetUserWithTasksAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.TasksCreated)
                .Include(u => u.TasksAssigned)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
        }
    }
}