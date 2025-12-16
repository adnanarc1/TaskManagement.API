
using TaskManagement.DataAccessLayer.Entities;


namespace TaskManagement.DataAccessLayer.Interfaces
{
    public interface ITaskItemRepository : IRepository<TaskItem>
    {
        //Task<IEnumerable<TaskItem>> GetAllWithDetailsAsync(TaskFilterDto filter, int userId, bool isAdmin);
        Task<IEnumerable<TaskItem>> GetTaskItemsWithDetailsAsync();
        Task<TaskItem> GetTaskItemWithDetailsAsync(int id);
        Task<IEnumerable<TaskItem>> GetUserTaskItemsAsync(int userId, bool includeAssigned = true);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(int statusId);
        Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(int priorityId);
        Task<IEnumerable<TaskItem>> GetTasksByTypeAsync(int typeId);
        Task<IEnumerable<TaskItem>> GetTasksWithFiltersAsync(
            string search = null,
            int? typeId = null,
            int? statusId = null,
            int? priorityId = null,
            int? createdBy = null,
            int? assignedTo = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
        Task<IEnumerable<TaskItem>> GetCompletedTasksAsync(DateTime? fromDate = null);
        Task<int> GetTaskCountByUserAsync(int userId);
        Task<bool> IsTaskAssignedToUserAsync(int taskId, int userId);
    }
}