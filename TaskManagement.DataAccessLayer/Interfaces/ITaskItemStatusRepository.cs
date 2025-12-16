using TaskManagement.DataAccessLayer.Entities;

namespace TaskManagement.DataAccessLayer.Interfaces
{
    public interface ITaskItemStatusRepository : IRepository<TaskItemStatus>
    {
        Task<TaskItemStatus> GetDefaultStatusAsync();
    }
}
