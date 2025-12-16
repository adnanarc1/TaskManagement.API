using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccessLayer.Entities;
using TaskManagement.DataAccessLayer.Data;



namespace TaskManagement.DataAccessLayer.Repositories
{
    public class PriorityRepository : Repository<Priority>, IPriorityRepository
    {
        public PriorityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Priority> GetDefaultPriorityAsync()
        {
            return await _context.Priorities
                .FirstOrDefaultAsync(p => p.Name == "Medium");
        }

        public async Task<Priority> GetByLevelAsync(int level)
        {
            return await _context.Priorities
                .FirstOrDefaultAsync(p => p.Level == level);
        }
    }
}