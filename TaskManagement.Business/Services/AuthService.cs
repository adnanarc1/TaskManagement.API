using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using TaskManagement.DataAccessLayer.Interfaces;
using TaskManagement.DataAccessLayer.Entities;



namespace TaskManagement.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning($"Authentication failed: User with email {email} not found");
                return null;
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    _logger.LogWarning($"Authentication failed: Invalid password for user {email}");
                    return null;
                }
            }

            _logger.LogInformation($"User {email} authenticated successfully");
            return user;
        }

        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            if (await _unitOfWork.Users.UserExistsAsync(registerDto.Email))
            {
                throw new ApplicationException($"Email {registerDto.Email} is already registered");
            }

            using var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            var passwordSalt = hmac.Key;

            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = registerDto.IsAdmin ? "Admin" : "User";

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"User {registerDto.Email} registered successfully");
            return user;
        }

        public string GenerateJwtToken(User user)
        {
            return _tokenService.GenerateToken(user);
        }
    }
}

