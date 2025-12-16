using TaskManagement.DataAccessLayer.Entities;
using TaskManagement.DataAccessLayer.Interfaces;

public interface IPriorityRepository : IRepository<Priority>
{
    Task<Priority> GetDefaultPriorityAsync();
    Task<Priority> GetByLevelAsync(int level);
}