// TaskManagement.DataAccessLayer/Repositories/TaskRepository.cs
using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccessLayer.Entities;
using TaskManagement.DataAccessLayer.Interfaces;
using TaskManagement.DataAccessLayer.Data;

namespace TaskManagement.DataAccessLayer.Repositories
{
    public class TaskItemRepository : Repository<TaskItem>, ITaskItemRepository
    {
        public TaskItemRepository(ApplicationDbContext context) : base(context)
        {
        }
        //public async Task<IEnumerable<TaskItem>> GetAllWithDetailsAsync(TaskFilterDto filter, int userId, bool isAdmin)
        //{
        //    var query = _context.TaskItems
        //        .Include(t => t.Type)
        //        .Include(t => t.Status)
        //        .Include(t => t.Priority)
        //        .Include(t => t.Creator)
        //        .Include(t => t.Assignee)
        //        .AsQueryable();

        //    // Apply role-based filtering
        //    if (!isAdmin)
        //    {
        //        query = query.Where(t => t.CreatedBy == userId || t.AssignedTo == userId);
        //    }

        //    // Apply search filter
        //    //if (!string.IsNullOrEmpty(filter?.Search))
        //    //{
        //    //    var search = filter.Search.ToLower();
        //    //    query = query.Where(t =>
        //    //        t.Name.ToLower().Contains(search) ||
        //    //        t.Description.ToLower().Contains(search));
        //    //}

        //    // Apply other filters
        //    if (filter?.TypeId.HasValue == true)
        //        query = query.Where(t => t.TypeId == filter.TypeId.Value);

        //    if (filter?.StatusId.HasValue == true)
        //        query = query.Where(t => t.StatusId == filter.StatusId.Value);

        //    if (filter?.PriorityId.HasValue == true)
        //        query = query.Where(t => t.PriorityId == filter.PriorityId.Value);

        //    if (filter?.AssignedTo.HasValue == true)
        //        query = query.Where(t => t.AssignedTo == filter.AssignedTo.Value);

        //    if (filter?.CreatedBy.HasValue == true)
        //        query = query.Where(t => t.CreatedBy == filter.CreatedBy.Value);

        //    // Apply ordering
        //    query = query
        //        .OrderByDescending(t => t.Priority.Level)
        //        .ThenBy(t => t.DueDate)
        //        .ThenByDescending(t => t.CreatedOn);

        //    return await query.ToListAsync();
        //}


        public async Task<IEnumerable<TaskItem>> GetTaskItemsWithDetailsAsync()
        {
            return await _context.TaskItems
                .Include(t => t.Type)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Where(t => t.StatusId != 4) // Exclude deleted TaskItems
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }

        public async Task<TaskItem> GetTaskItemWithDetailsAsync(int id)
        {
            return await _context.TaskItems
                .Include(t => t.Type)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.Id == id && t.StatusId != 4);
        }

        public async Task<IEnumerable<TaskItem>> GetUserTaskItemsAsync(int userId, bool includeAssigned = true)
        {
            var query = _context.TaskItems
                .Include(t => t.Type)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Where(t => t.StatusId != 4);

            if (includeAssigned)
            {
                query = query.Where(t => t.CreatedBy == userId || t.AssignedTo == userId);
            }
            else
            {
                query = query.Where(t => t.CreatedBy == userId);
            }

            return await query
                .OrderByDescending(t => t.Priority.Level)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(int statusId)
        {
            return await _context.TaskItems
                .Include(t => t.Type)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Where(t => t.StatusId == statusId && t.StatusId != 4)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(int priorityId)
        {
            return await _context.TaskItems
                .Include(t => t.Type)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Where(t => t.PriorityId == priorityId && t.StatusId != 4)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByTypeAsync(int typeId)
        {
            return await _context.TaskItems
                .Include(t => t.Type)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Where(t => t.TypeId == typeId && t.StatusId != 4)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksWithFiltersAsync(
            string search = null,
            int? typeId = null,
            int? statusId = null,
            int? priorityId = null,
            int? createdBy = null,
            int? assignedTo = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null)
        {
            var query = _context.TaskItems
                .Include(t => t.Type)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Where(t => t.StatusId != 4);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t =>
                    t.Name.Contains(search) ||
                    t.Description.Contains(search));
            }

            if (typeId.HasValue)
                query = query.Where(t => t.TypeId == typeId.Value);

            if (statusId.HasValue)
                query = query.Where(t => t.StatusId == statusId.Value);

            if (priorityId.HasValue)
                query = query.Where(t => t.PriorityId == priorityId.Value);

            if (createdBy.HasValue)
                query = query.Where(t => t.CreatedBy == createdBy.Value);

            if (assignedTo.HasValue)
                query = query.Where(t => t.AssignedTo == assignedTo.Value);

            if (dueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= dueDateFrom.Value);

            if (dueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= dueDateTo.Value);

            return await query
                .OrderByDescending(t => t.Priority.Level)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
        {
            return await _context.TaskItems
                .Include(t => t.Type)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Where(t => t.StatusId != 4 &&
                           t.StatusId != 3 && // Not completed
                           t.DueDate.HasValue &&
                           t.DueDate < DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetCompletedTasksAsync(DateTime? fromDate = null)
        {
            var query = _context.TaskItems
                .Include(t => t.Type)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Where(t => t.StatusId == 3); // Completed

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CompletedDate >= fromDate.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetTaskCountByUserAsync(int userId)
        {
            return await _context.TaskItems
                .CountAsync(t => (t.CreatedBy == userId || t.AssignedTo == userId) &&
                                t.StatusId != 4);
        }

        public async Task<bool> IsTaskAssignedToUserAsync(int taskId, int userId)
        {
            return await _context.TaskItems
                .AnyAsync(t => t.Id == taskId &&
                              t.StatusId != 4 &&
                              (t.CreatedBy == userId || t.AssignedTo == userId));
        }
    }
}