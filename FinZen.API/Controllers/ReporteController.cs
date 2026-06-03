using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinZen.API.Data;
using FinZen.API.DTOs;
using FinZen.API.Models; // <-- El fix del Build

namespace FinZen.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReporteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReporteController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/reporte/top-usuarios
        [HttpGet("top-usuarios")]
        public async Task<IActionResult> GetTopUsuarios()
        {
            var topUsuarios = await _context.Metas
                .Where(m =>
                    m.FechaCompletada.HasValue &&
                    m.MontoActual >= m.MontoObjetivo) // <-- Fix para Entity Framework
                .Include(m => m.Usuario)
                .GroupBy(m => new { m.UsuarioId, m.Usuario.Nombre, m.Usuario.Email })
                .Select(g => new TopUsuarioDTO
                {
                    NombreUsuario = g.Key.Nombre,
                    Email = g.Key.Email,
                    MetasCompletadas = g.Count(),
                    PromedioDiasAdelantado = g.Average(m =>
                        (m.FechaLimite - m.FechaCompletada!.Value).Days)
                })
                .OrderByDescending(u => u.PromedioDiasAdelantado)
                .Take(3)
                .ToListAsync();

            return Ok(topUsuarios);
        }

        // POST api/reporte/generar-datos-prueba
        [HttpPost("generar-datos-prueba")]
        [AllowAnonymous] 
        public async Task<IActionResult> GenerarDatosPrueba()
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == "pro1@finzen.com"))
                return Ok(new { mensaje = "Los datos de prueba ya fueron generados." });

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Test1234!");

            var user1 = new Usuario { Nombre = "Ana Experta", Email = "pro1@finzen.com", PasswordHash = passwordHash };
            var user2 = new Usuario { Nombre = "Beto Ahorros", Email = "pro2@finzen.com", PasswordHash = passwordHash };
            var user3 = new Usuario { Nombre = "Carlos Lento", Email = "pro3@finzen.com", PasswordHash = passwordHash };

            _context.Usuarios.AddRange(user1, user2, user3);
            await _context.SaveChangesAsync(); 

            var metasPrueba = new List<Meta>
            {
                new Meta { 
                    UsuarioId = user1.Id, Nombre = "Fondo de Emergencia", MontoObjetivo = 1000, MontoActual = 1000, Prioridad = 5,
                    FechaLimite = DateTime.UtcNow.AddDays(5), 
                    FechaCompletada = DateTime.UtcNow.AddDays(-5) 
                },
                new Meta { 
                    UsuarioId = user2.Id, Nombre = "Laptop Nueva", MontoObjetivo = 800, MontoActual = 800, Prioridad = 4,
                    FechaLimite = DateTime.UtcNow.AddDays(10), 
                    FechaCompletada = DateTime.UtcNow.AddDays(7) 
                },
                new Meta { 
                    UsuarioId = user3.Id, Nombre = "Viaje", MontoObjetivo = 500, MontoActual = 500, Prioridad = 3,
                    FechaLimite = DateTime.UtcNow.AddDays(-2), 
                    FechaCompletada = DateTime.UtcNow.AddDays(-2) 
                }
            };

            _context.Metas.AddRange(metasPrueba);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "¡Datos inyectados con éxito! Prueba el Top 3." });
        }
    }
}