// TaskManagement.DataAccessLayer/Repositories/MasterDataRepositories.cs
using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccessLayer.Entities;
using TaskManagement.DataAccessLayer.Interfaces;
using TaskManagement.DataAccessLayer.Data;

namespace TaskManagement.DataAccessLayer.Repositories
{
    public class TaskTypeRepository : Repository<TaskType>, ITaskTypeRepository
    {
        public TaskTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskType>> GetTypesWithParentAsync()
        {
            return await _context.TaskTypes
                .Include(t => t.Parent)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskType>> GetChildTypesAsync(int parentId)
        {
            return await _context.TaskTypes
                .Where(t => t.ParentId == parentId)
                .ToListAsync();
        }
    }


  
}