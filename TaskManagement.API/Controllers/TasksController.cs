
// TasksController.cs
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Business.Services;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            ITaskService taskService,
            IMapper mapper,
            ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _mapper = mapper;
            _logger = logger;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(
        //    [FromQuery] string search = null,
        //    [FromQuery] int? typeId = null,
        //    [FromQuery] int? statusId = null,
        //    [FromQuery] int? priorityId = null)
        //{
        //    try
        //    {
        //        var filter = new TaskFilterDto
        //        {
        //            Search = search,
        //            TypeId = typeId,
        //            StatusId = statusId,
        //            PriorityId = priorityId
        //        };

        //        var tasks = await _taskService.GetTasksAsync(
        //            filter,
        //            GetUserId(),
        //            IsAdmin());

        //        return Ok(tasks);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting tasks");
        //        return StatusCode(500, new { message = "Internal server error" });
        //    }
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(
                    id,
                    GetUserId(),
                    IsAdmin());

                if (task == null)
                    return NotFound(new { message = $"Task with id {id} not found or unauthorized" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting task with id {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto createTaskDto)
        {
            try
            {
                var taskDto = await _taskService.CreateTaskAsync(
                    createTaskDto,
                    GetUserId());

                return CreatedAtAction(
                    nameof(GetTask),
                    new { id = taskDto.Id },
                    taskDto);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, UpdateTaskDto updateTaskDto)
        {
            try
            {
                var taskDto = await _taskService.UpdateTaskAsync(
                    id,
                    updateTaskDto,
                    GetUserId(),
                    IsAdmin());

                if (taskDto == null)
                    return NotFound(new { message = $"Task with id {id} not found or unauthorized" });

                return Ok(taskDto);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating task with id {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(
                    id,
                    GetUserId(),
                    IsAdmin());

                if (!result)
                    return NotFound(new { message = $"Task with id {id} not found or unauthorized" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting task with id {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}






//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;
//using TaskManagement.DataAccessLayer.Entities;
//using TaskManagement.DataAccessLayer.Data;

//namespace TaskManagement.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    public class TasksController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public TasksController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(
//            [FromQuery] string search = null,
//            [FromQuery] int? typeId = null,
//            [FromQuery] int? statusId = null,
//            [FromQuery] int? priorityId = null)
//        {
//            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
//            var isAdmin = User.IsInRole("Admin");

//            var query = _context.TaskItems
//                .Include(t => t.Type)
//                .Include(t => t.Status)
//                .Include(t => t.Priority)
//                .Include(t => t.Creator)
//                .Include(t => t.Assignee)
//                .AsQueryable();

//            // Apply filters based on user role
//            if (!isAdmin)
//            {
//                query = query.Where(t => t.CreatedBy == userId || t.AssignedTo == userId);
//            }

//            // Apply search filter
//            if (!string.IsNullOrEmpty(search))
//            {
//                query = query.Where(t => t.Name.Contains(search) ||
//                                         t.Description.Contains(search));
//            }

//            // Apply other filters
//            if (typeId.HasValue)
//                query = query.Where(t => t.TypeId == typeId.Value);

//            if (statusId.HasValue)
//                query = query.Where(t => t.StatusId == statusId.Value);

//            if (priorityId.HasValue)
//                query = query.Where(t => t.PriorityId == priorityId.Value);

//            var tasks = await query
//                .OrderByDescending(t => t.Priority.Level)
//                .ThenBy(t => t.DueDate)
//                .ToListAsync();

//            return Ok(tasks.Select(t => MapToDto(t)));
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<TaskDto>> GetTask(int id)
//        {
//            var task = await _context.TaskItems
//                .Include(t => t.Type)
//                .Include(t => t.Status)
//                .Include(t => t.Priority)
//                .Include(t => t.Creator)
//                .Include(t => t.Assignee)
//                .FirstOrDefaultAsync(t => t.Id == id);

//            if (task == null) return NotFound();

//            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
//            var isAdmin = User.IsInRole("Admin");

//            if (!isAdmin && task.CreatedBy != userId && task.AssignedTo != userId)
//                return Forbid();

//            return Ok(MapToDto(task));
//        }

//        [HttpPost]
//        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto taskDto)
//        {
//            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

//            var task = new TaskItem
//            {
//                Name = taskDto.Name,
//                Description = taskDto.Description,
//                TypeId = taskDto.TypeId,
//                StatusId = taskDto.StatusId,
//                PriorityId = taskDto.PriorityId,
//                CreatedBy = userId,
//                AssignedTo = taskDto.AssignedTo,
//                DueDate = taskDto.DueDate,
//                CreatedOn = DateTime.UtcNow
//            };

//            _context.TaskItems.Add(task);
//            await _context.SaveChangesAsync();

//            var createdTask = await _context.TaskItems
//                .Include(t => t.Type)
//                .Include(t => t.Status)
//                .Include(t => t.Priority)
//                .Include(t => t.Creator)
//                .Include(t => t.Assignee)
//                .FirstOrDefaultAsync(t => t.Id == task.Id);

//            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, MapToDto(createdTask));
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto taskDto)
//        {
//            var task = await _context.TaskItems.FindAsync(id);
//            if (task == null) return NotFound();

//            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
//            var isAdmin = User.IsInRole("Admin");

//            if (!isAdmin && task.CreatedBy != userId)
//                return Forbid();

//            task.Name = taskDto.Name;
//            task.Description = taskDto.Description;
//            task.TypeId = taskDto.TypeId;
//            task.StatusId = taskDto.StatusId;
//            task.PriorityId = taskDto.PriorityId;
//            task.AssignedTo = taskDto.AssignedTo;
//            task.DueDate = taskDto.DueDate;
//            task.ModifiedBy = userId;
//            task.ModifiedOn = DateTime.UtcNow;

//            if (taskDto.StatusId == 3) // Completed
//                task.CompletedDate = DateTime.UtcNow;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteTask(int id)
//        {
//            var task = await _context.TaskItems.FindAsync(id);
//            if (task == null) return NotFound();

//            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
//            var isAdmin = User.IsInRole("Admin");

//            if (!isAdmin && task.CreatedBy != userId)
//                return Forbid();

//            task.StatusId = 4; // Deleted status
//            task.ModifiedBy = userId;
//            task.ModifiedOn = DateTime.UtcNow;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        private TaskDto MapToDto(TaskItem task)
//        {
//            return new TaskDto
//            {
//                Id = task.Id,
//                Name = task.Name,
//                Description = task.Description,
//                TypeId = task.TypeId,
//                TypeName = task.Type?.Name,
//                StatusId = task.StatusId,
//                StatusName = task.Status?.Name,
//                PriorityId = task.PriorityId,
//                PriorityName = task.Priority?.Name,
//                CreatedBy = task.CreatedBy,
//                CreatedByName = $"{task.Creator?.FirstName} {task.Creator?.LastName}",
//                AssignedTo = task.AssignedTo,
//                AssignedToName = task.Assignee != null ?
//                    $"{task.Assignee.FirstName} {task.Assignee.LastName}" : null,
//                DueDate = task.DueDate,
//                CreatedOn = task.CreatedOn,
//                ModifiedOn = task.ModifiedOn,
//                CompletedDate = task.CompletedDate
//            };
//        }
//    }
//}