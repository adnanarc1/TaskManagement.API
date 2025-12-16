namespace TaskManagement.Business.Services
{
    public interface ITaskService
    {
        //Task<IEnumerable<TaskDto>> GetTasksAsync(TaskFilterDto filter, int userId, bool isAdmin);
        Task<TaskDto> GetTaskByIdAsync(int id, int userId, bool isAdmin);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId);
        Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId, bool isAdmin);
        Task<bool> DeleteTaskAsync(int id, int userId, bool isAdmin);
        Task<bool> TaskExistsAsync(int id);
    }
}