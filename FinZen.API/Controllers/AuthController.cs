using Microsoft.AspNetCore.Mvc;
using FinZen.API.DTOs;
using FinZen.API.Services;

namespace FinZen.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var (resultado, error) = await _authService.Register(dto);

            if (error != null)
                return BadRequest(new { mensaje = error });

            return Ok(resultado);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var resultado = await _authService.Login(dto);

            if (resultado == null)
                return Unauthorized(new { mensaje = "Email o contraseña incorrectos" });

            return Ok(resultado);
        }
    }
}