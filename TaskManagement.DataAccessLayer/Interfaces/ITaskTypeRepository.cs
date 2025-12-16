
using TaskManagement.DataAccessLayer.Entities;

namespace TaskManagement.DataAccessLayer.Interfaces
{
    public interface ITaskTypeRepository : IRepository<TaskType>
    {
        Task<IEnumerable<TaskType>> GetTypesWithParentAsync();
        Task<IEnumerable<TaskType>> GetChildTypesAsync(int parentId);
    }
}