// TaskManagement.DataAccessLayer/Repositories/UnitOfWork.cs
using TaskManagement.DataAccessLayer.Interfaces;
using TaskManagement.DataAccessLayer.Data;

namespace TaskManagement.DataAccessLayer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IUserRepository _users;
        private ITaskItemRepository _tasks;
        private ITaskTypeRepository _taskTypes;
        private ITaskItemStatusRepository _taskStatuses;
        private IPriorityRepository _priorities;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public ITaskItemRepository Tasks =>
            _tasks ??= new TaskItemRepository(_context);

        public ITaskTypeRepository TaskTypes =>
            _taskTypes ??= new TaskTypeRepository(_context);

        public ITaskItemStatusRepository TaskStatuses =>
            _taskStatuses ??= new TaskItemStatusRepository(_context);

        public IPriorityRepository Priorities =>
            _priorities ??= new PriorityRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}