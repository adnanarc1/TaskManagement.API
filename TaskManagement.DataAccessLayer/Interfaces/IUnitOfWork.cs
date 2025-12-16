
namespace TaskManagement.DataAccessLayer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ITaskItemRepository Tasks { get; }
        ITaskTypeRepository TaskTypes { get; }
        ITaskItemStatusRepository TaskStatuses { get; }
        IPriorityRepository Priorities { get; }

        Task<int> CompleteAsync();
        int Complete();
    }
}