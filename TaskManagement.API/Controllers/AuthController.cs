using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Business.Services;
using TaskManagement.DataAccessLayer.Interfaces;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IMapper mapper,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            try
            {
                var user = await _authService.RegisterAsync(registerDto);
                var token = _authService.GenerateJwtToken(user);

                var userDto = _mapper.Map<UserDto>(user);
                userDto.Token = token;

                return Ok(userDto);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _authService.AuthenticateAsync(loginDto.Email, loginDto.Password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var token = _authService.GenerateJwtToken(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Token = token;

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}