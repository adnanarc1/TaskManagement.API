// TaskService.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagement.DataAccessLayer.Interfaces;
using TaskManagement.DataAccessLayer.Entities;

namespace TaskManagement.Business.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TaskService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskDto>> GetTasksAsync()
        {
            try
            {
                var tasks = await _unitOfWork.Tasks.GetTaskItemsWithDetailsAsync();
                return _mapper.Map<IEnumerable<TaskDto>>(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                throw;
            }
        }

        public async Task<TaskDto> GetTaskByIdAsync(int id, int userId, bool isAdmin)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetTaskItemWithDetailsAsync(id);

                if (task == null)
                {
                    _logger.LogWarning($"Task with id {id} not found");
                    return null;
                }

                // Authorization check
                if (!isAdmin && task.CreatedBy != userId && task.AssignedTo != userId)
                {
                    _logger.LogWarning($"User {userId} is not authorized to view task {id}");
                    return null;
                }

                return _mapper.Map<TaskDto>(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting task with id {id}");
                throw;
            }
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId)
        {
            try
            {
                // Validate references exist
                //if (!await _unitOfWork.TaskTypes.ExistsAsync(createTaskDto.TypeId))
                //{
                //    throw new ApplicationException($"Task type with id {createTaskDto.TypeId} not found");
                //}

                //if (!await _unitOfWork.TaskStatuses.ExistsAsync(createTaskDto.StatusId))
                //{
                //    throw new ApplicationException($"Task status with id {createTaskDto.StatusId} not found");
                //}

                //if (!await _unitOfWork.TaskPriorities.ExistsAsync(createTaskDto.PriorityId))
                //{
                //    throw new ApplicationException($"Task priority with id {createTaskDto.PriorityId} not found");
                //}

                //if (createTaskDto.AssignedTo.HasValue &&
                //    !await _unitOfWork.Users.ExistsAsync(createTaskDto.AssignedTo.Value))
                //{
                //    throw new ApplicationException($"Assigned user with id {createTaskDto.AssignedTo} not found");
                //}

                // Map DTO to entity
                var task = _mapper.Map<TaskItem>(createTaskDto);
                task.CreatedBy = userId;
                task.CreatedOn = DateTime.UtcNow;

                // Add task
                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.CompleteAsync();

                // Get the task with all details
                var createdTask = await _unitOfWork.Tasks.GetTaskItemWithDetailsAsync(task.Id);

                _logger.LogInformation($"Task {task.Id} created by user {userId}");

                return _mapper.Map<TaskDto>(createdTask);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, $"Failed to create task: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        public async Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId, bool isAdmin)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(id);

                if (task == null)
                {
                    _logger.LogWarning($"Task with id {id} not found");
                    return null;
                }

                // Authorization check
                if (!isAdmin && task.CreatedBy != userId)
                {
                    _logger.LogWarning($"User {userId} is not authorized to update task {id}");
                    return null;
                }

                // Validate references exist
                //if (!await _unitOfWork.TaskTypes.ExistsAsync(updateTaskDto.TypeId))
                //{
                //    throw new ApplicationException($"Task type with id {updateTaskDto.TypeId} not found");
                //}

                //if (!await _unitOfWork.TaskStatuses.ExistsAsync(updateTaskDto.StatusId))
                //{
                //    throw new ApplicationException($"Task status with id {updateTaskDto.StatusId} not found");
                //}

                //if (!await _unitOfWork.TaskPriorities.ExistsAsync(updateTaskDto.PriorityId))
                //{
                //    throw new ApplicationException($"Task priority with id {updateTaskDto.PriorityId} not found");
                //}

                //if (updateTaskDto.AssignedTo &&
                //    !await _unitOfWork.Users.ExistsAsync(updateTaskDto.AssignedTo))
                //{
                //    throw new ApplicationException($"Assigned user with id {updateTaskDto.AssignedTo} not found");
                //}

                // Update properties
                task.Name = updateTaskDto.Name;
                task.Description = updateTaskDto.Description;
                task.TypeId = updateTaskDto.TypeId;
                task.StatusId = updateTaskDto.StatusId;
                task.PriorityId = updateTaskDto.PriorityId;
                task.AssignedTo = updateTaskDto.AssignedTo;
                task.DueDate = updateTaskDto.DueDate;
                task.ModifiedBy = userId;
                task.ModifiedOn = DateTime.UtcNow;

                // Set completed date if status is completed
                if (updateTaskDto.StatusId == 3) // Assuming 3 is "Completed"
                {
                    task.CompletedDate = DateTime.UtcNow;
                }

                // Save changes
                await _unitOfWork.CompleteAsync();

                // Get updated task with details
                var updatedTask = await _unitOfWork.Tasks.GetTaskItemWithDetailsAsync(id);

                _logger.LogInformation($"Task {id} updated by user {userId}");

                return _mapper.Map<TaskDto>(updatedTask);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, $"Failed to update task {id}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating task with id {id}");
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id, int userId, bool isAdmin)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(id);

                if (task == null)
                {
                    _logger.LogWarning($"Task with id {id} not found");
                    return false;
                }

                // Authorization check
                if (!isAdmin && task.CreatedBy != userId)
                {
                    _logger.LogWarning($"User {userId} is not authorized to delete task {id}");
                    return false;
                }

                // Soft delete by changing status to "Deleted" (assuming status 4 is "Deleted")
                task.StatusId = 4;
                task.ModifiedBy = userId;
                task.ModifiedOn = DateTime.UtcNow;

                // Save changes
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Task {id} deleted by user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting task with id {id}");
                throw;
            }
        }

        public Task<bool> TaskExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        //public async Task<bool> TaskExistsAsync(int id)
        //{
        //    return await _unitOfWork.Tasks.ExistsAsync(id);
        //}

        //public async Task<IEnumerable<TaskDto>> GetTasksAsync(TaskFilterDto filter, int userId, bool isAdmin)
        //{
        //    try
        //    {
        //        var tasks = await _unitOfWork.Tasks.GetAllWithDetailsAsync(filter, userId, isAdmin);
        //        return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting tasks");
        //        throw;
        //    }
        //}

    }
}