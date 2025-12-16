// UserService.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using TaskManagement.DataAccessLayer.Interfaces;
using TaskManagement.DataAccessLayer.Entities;

namespace TaskManagement.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetActiveUsersAsync();
                return _mapper.Map<IEnumerable<UserDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);

                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning($"User with id {id} not found or inactive");
                    return null;
                }

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with id {id}");
                throw;
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                // Check if user with email already exists
                if (await _unitOfWork.Users.UserExistsAsync(createUserDto.Email))
                {
                    throw new ApplicationException($"Email {createUserDto.Email} is already registered");
                }

                // Hash password
                using var hmac = new HMACSHA512();
                var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(createUserDto.Password));
                var passwordSalt = hmac.Key;

                // Map DTO to entity
                var user = _mapper.Map<User>(createUserDto);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Email = createUserDto.Email;
                user.CreatedDate = DateTime.UtcNow;
                user.IsActive = true;

                // Add user
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"User {createUserDto.Email} created successfully");

                return _mapper.Map<UserDto>(user);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, $"Failed to create user: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning($"User with id {id} not found");
                    return null;
                }

                // Update properties
                _mapper.Map(updateUserDto, user);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"User {id} updated successfully");

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with id {id}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning($"User with id {id} not found");
                    return false;
                }

                // Soft delete
                user.IsActive = false;

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"User {id} deleted (soft) successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with id {id}");
                throw;
            }
        }

        //public async Task<bool> UserExistsAsync(int id)
        //{
        //    return await _unitOfWork.Users.ExistsAsync(id);
        //}

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _unitOfWork.Users.UserExistsAsync(email);
        }
    }
}