using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FinZen.API.Data;
using FinZen.API.DTOs;
using FinZen.API.Models;

namespace FinZen.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransaccionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransaccionController(AppDbContext context)
        {
            _context = context;
        }

        private int ObtenerUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarioId = ObtenerUsuarioId();

            var transacciones = await _context.Transacciones
                .Include(t => t.Categoria)
                .Where(t => t.UsuarioId == usuarioId)
                .Select(t => new TransaccionResponseDTO
                {
                    Id = t.Id,
                    Descripcion = t.Descripcion,
                    Monto = t.Monto,
                    Tipo = t.Tipo.ToString(),
                    CategoriaNombre = t.Categoria.Nombre,
                    Fecha = t.Fecha
                })
                .ToListAsync();

            return Ok(transacciones);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearTransaccionDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();

            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.Id == dto.CategoriaId);

            if (!categoriaExiste)
                return BadRequest(new { mensaje = "La categoría no existe" });

            var transaccion = new Transaccion
            {
                Descripcion = dto.Descripcion,
                Monto = dto.Monto,
                Tipo = Enum.Parse<TipoTransaccion>(dto.Tipo),
                CategoriaId = dto.CategoriaId,
                UsuarioId = usuarioId
            };

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();

            await _context.Entry(transaccion)
                .Reference(t => t.Categoria)
                .LoadAsync();

            return Ok(new TransaccionResponseDTO
            {
                Id = transaccion.Id,
                Descripcion = transaccion.Descripcion,
                Monto = transaccion.Monto,
                Tipo = transaccion.Tipo.ToString(),
                CategoriaNombre = transaccion.Categoria.Nombre,
                Fecha = transaccion.Fecha
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CrearTransaccionDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();

            var transaccion = await _context.Transacciones
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transaccion == null)
                return NotFound(new { mensaje = "Transacción no encontrada" });

            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.Id == dto.CategoriaId);

            if (!categoriaExiste)
                return BadRequest(new { mensaje = "La categoría no existe" });

            transaccion.Descripcion = dto.Descripcion;
            transaccion.Monto = dto.Monto;
            transaccion.Tipo = Enum.Parse<TipoTransaccion>(dto.Tipo);
            transaccion.CategoriaId = dto.CategoriaId;

            await _context.SaveChangesAsync();

            return Ok(new TransaccionResponseDTO
            {
                Id = transaccion.Id,
                Descripcion = transaccion.Descripcion,
                Monto = transaccion.Monto,
                Tipo = transaccion.Tipo.ToString(),
                CategoriaNombre = transaccion.Categoria.Nombre,
                Fecha = transaccion.Fecha
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = ObtenerUsuarioId();

            var transaccion = await _context.Transacciones
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transaccion == null)
                return NotFound(new { mensaje = "Transacción no encontrada" });

            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Transacción eliminada correctamente" });
        }
    }
}