using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccessLayer.Entities;
using TaskManagement.DataAccessLayer.Interfaces;
using TaskManagement.DataAccessLayer.Data;
using TaskManagement.DataAccessLayer.Repositories;

public class TaskItemStatusRepository : Repository<TaskItemStatus>, ITaskItemStatusRepository
{
    public TaskItemStatusRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TaskItemStatus> GetDefaultStatusAsync()
    {
        return await _context.TaskItemStatuses
            .FirstOrDefaultAsync(s => s.Name == "Pending");
    }
}
