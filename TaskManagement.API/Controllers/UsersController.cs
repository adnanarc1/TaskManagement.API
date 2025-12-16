
// UsersController.cs
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Business.Services;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Uncomment when ready
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                    return NotFound(new { message = $"User with id {id} not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with id {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            try
            {
                var userDto = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser),
                    new { id = userDto.Id },
                    userDto);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                var userDto = await _userService.UpdateUserAsync(id, updateUserDto);

                if (userDto == null)
                    return NotFound(new { message = $"User with id {id} not found" });

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with id {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);

                if (!result)
                    return NotFound(new { message = $"User with id {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with id {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}



//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Cryptography;
//using System.Text;
//using TaskManagement.DataAccessLayer.Entities;
//using TaskManagement.DataAccessLayer.Data;

//namespace TaskManagement.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    //[Authorize(Roles = "Admin")]
//    public class UsersController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public UsersController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
//        {
//            var users = await _context.Users
//                .Where(u => u.IsActive)
//                .Select(u => new UserDto
//                {
//                    Id = u.Id,
//                    FirstName = u.FirstName,
//                    LastName = u.LastName,
//                    Email = u.Email,
//                    PhoneNumber = u.PhoneNumber,
//                    Role = u.Role,
//                    CreatedDate = u.CreatedDate
//                })
//                .ToListAsync();

//            return Ok(users);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<UserDto>> GetUser(int id)
//        {
//            var user = await _context.Users
//                .Where(u => u.Id == id && u.IsActive)
//                .Select(u => new UserDto
//                {
//                    Id = u.Id,
//                    FirstName = u.FirstName,
//                    LastName = u.LastName,
//                    Email = u.Email,
//                    PhoneNumber = u.PhoneNumber,
//                    Role = u.Role,
//                    CreatedDate = u.CreatedDate
//                })
//                .FirstOrDefaultAsync();

//            if (user == null) return NotFound();
//            return Ok(user);
//        }

//        [HttpPost]
//        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto userDto)
//        {
//            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
//                return BadRequest("Email already exists");

//            using var hmac = new HMACSHA512();

//            var user = new User
//            {
//                FirstName = userDto.FirstName,
//                LastName = userDto.LastName,
//                Email = userDto.Email,
//                PhoneNumber = userDto.PhoneNumber,
//                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password)),
//                PasswordSalt = hmac.Key,
//                Role = userDto.Role
//            };

//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetUser),
//                new { id = user.Id },
//                new UserDto
//                {
//                    Id = user.Id,
//                    FirstName = user.FirstName,
//                    LastName = user.LastName,
//                    Email = user.Email,
//                    PhoneNumber = user.PhoneNumber,
//                    Role = user.Role
//                });
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto userDto)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null) return NotFound();

//            user.FirstName = userDto.FirstName;
//            user.LastName = userDto.LastName;
//            user.PhoneNumber = userDto.PhoneNumber;
//            user.Role = userDto.Role;
//            user.IsActive = userDto.IsActive;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteUser(int id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null) return NotFound();

//            user.IsActive = false;
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}