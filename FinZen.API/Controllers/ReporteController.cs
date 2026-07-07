using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinZen.API.Data;
using FinZen.API.DTOs;
using FinZen.API.Models;

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


        [HttpGet("top-usuarios")]
        public async Task<IActionResult> GetTopUsuarios()
        {
            var topUsuarios = await _context.Metas
                .Where(m =>
                    m.FechaCompletada.HasValue &&
                    m.MontoActual >= m.MontoObjetivo) 
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

        // Cuenta única con transacciones y metas variadas para explorar toda la app sin cargar datos a mano.
        [HttpPost("generar-cuenta-demo")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerarCuentaDemo()
        {
            const string email = "demo@finzen.com";
            const string password = "Demo1234!";

            var existente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (existente != null)
            {
                _context.Transacciones.RemoveRange(_context.Transacciones.Where(t => t.UsuarioId == existente.Id));
                _context.Metas.RemoveRange(_context.Metas.Where(m => m.UsuarioId == existente.Id));
                _context.Usuarios.Remove(existente);
                await _context.SaveChangesAsync();
            }

            var usuario = new Usuario
            {
                Nombre = "Usuario Demo",
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var ahora = DateTime.UtcNow;
            var inicioMes = new DateTime(ahora.Year, ahora.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var diasDisponibles = Math.Max((int)(ahora - inicioMes).TotalDays, 0);
            DateTime FechaDemo(int pasoDeseado) => inicioMes.AddDays(Math.Min(pasoDeseado, diasDisponibles));

            var transacciones = new List<Transaccion>
            {
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Salario", Monto = 1500, Tipo = TipoTransaccion.Ingreso, CategoriaId = 7, Fecha = FechaDemo(1) },
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Supermercado", Monto = 180, Tipo = TipoTransaccion.Gasto, CategoriaId = 1, Fecha = FechaDemo(2) },
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Proyecto freelance", Monto = 300, Tipo = TipoTransaccion.Ingreso, CategoriaId = 8, Fecha = FechaDemo(3) },
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Bus y taxi", Monto = 80, Tipo = TipoTransaccion.Gasto, CategoriaId = 2, Fecha = FechaDemo(4) },
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Consulta médica", Monto = 45, Tipo = TipoTransaccion.Gasto, CategoriaId = 4, Fecha = FechaDemo(5) },
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Cine", Monto = 60, Tipo = TipoTransaccion.Gasto, CategoriaId = 3, Fecha = FechaDemo(6) },
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Restaurante", Monto = 40, Tipo = TipoTransaccion.Gasto, CategoriaId = 1, Fecha = FechaDemo(7) },
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Curso online", Monto = 100, Tipo = TipoTransaccion.Gasto, CategoriaId = 5, Fecha = FechaDemo(8) },
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Internet y luz", Monto = 150, Tipo = TipoTransaccion.Gasto, CategoriaId = 6, Fecha = FechaDemo(9) },
                new Transaccion { UsuarioId = usuario.Id, Descripcion = "Regalo de cumpleaños", Monto = 40, Tipo = TipoTransaccion.Gasto, CategoriaId = 10, Fecha = FechaDemo(diasDisponibles) },
            };
            _context.Transacciones.AddRange(transacciones);

            var metas = new List<Meta>
            {
                new Meta { UsuarioId = usuario.Id, Nombre = "Fondo de Emergencia", Descripcion = "Colchón para imprevistos", MontoObjetivo = 2000, MontoActual = 2000, Prioridad = 5, FechaLimite = ahora.AddDays(-1), FechaCompletada = ahora.AddDays(-10) },
                new Meta { UsuarioId = usuario.Id, Nombre = "Vacaciones", Descripcion = "Viaje de fin de año", MontoObjetivo = 1500, MontoActual = 600, Prioridad = 3, FechaLimite = ahora.AddDays(60) },
                new Meta { UsuarioId = usuario.Id, Nombre = "Laptop nueva", Descripcion = "Para el trabajo", MontoObjetivo = 1200, MontoActual = 300, Prioridad = 4, FechaLimite = ahora.AddDays(30) },
                new Meta { UsuarioId = usuario.Id, Nombre = "Curso de inglés", Descripcion = "Certificación B2", MontoObjetivo = 500, MontoActual = 450, Prioridad = 2, FechaLimite = ahora.AddDays(10) },
            };
            _context.Metas.AddRange(metas);

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Cuenta demo lista: {email} / {password}" });
        }
    }
}