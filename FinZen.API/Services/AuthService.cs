using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinZen.API.Data;
using FinZen.API.DTOs;
using FinZen.API.Models;

namespace FinZen.API.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string? ValidarPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "La contraseña no puede estar vacía";

            if (password.Length < 8)
                return "La contraseña debe tener al menos 8 caracteres";

            if (!password.Any(char.IsUpper))
                return "La contraseña debe tener al menos una mayúscula";

            if (!password.Any(char.IsDigit))
                return "La contraseña debe tener al menos un número";

            return null;
        }

        public async Task<(AuthResponseDTO? respuesta, string? error)> Register(RegisterDTO dto)
        {
            // Validar contraseña
            var errorPassword = ValidarPassword(dto.Password);
            if (errorPassword != null)
                return (null, errorPassword);

            // Validar email único
            bool emailExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExiste)
                return (null, "El email ya está registrado");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return (new AuthResponseDTO
            {
                Token = GenerarToken(usuario),
                Nombre = usuario.Nombre,
                Email = usuario.Email
            }, null);
        }

        public async Task<AuthResponseDTO?> Login(LoginDTO dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null)
                return null;

            bool passwordCorrecta = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);

            if (!passwordCorrecta)
                return null;

            return new AuthResponseDTO
            {
                Token = GenerarToken(usuario),
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        private string GenerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nombre)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    double.Parse(_configuration["Jwt:ExpiresInHours"]!)
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}