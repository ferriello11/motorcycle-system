using Microsoft.AspNetCore.Mvc;
using TesteTecnico.Application.Interfaces;
using TesteTecnico.Application.DTOs;

namespace TesteTecnico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = await _userService.RegisterAsync(dto.Name, dto.Email, dto.Password, dto.Role);
            return Ok(new { user.Id, user.Name, user.Email, user.Role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto.Email, dto.Password);
            if (token == null) return Unauthorized(new { message = "Credenciais inválidas" });

            return Ok(new { token });
        }
    }
    
}
