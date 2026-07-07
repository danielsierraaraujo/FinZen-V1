using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinZen.API.DTOs;
using FinZen.API.Interfaces;
using FinZen.API.Models;

namespace FinZen.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransaccionController : ControllerBase
    {
        private readonly ITransaccionRepository _transaccionRepository;

        public TransaccionController(ITransaccionRepository transaccionRepository)
        {
            _transaccionRepository = transaccionRepository;
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
            var transacciones = await _transaccionRepository.GetByUsuarioId(usuarioId);

            return Ok(transacciones.Select(t => new TransaccionResponseDTO
            {
                Id = t.Id,
                Descripcion = t.Descripcion,
                Monto = t.Monto,
                Tipo = t.Tipo.ToString(),
                CategoriaNombre = t.Categoria.Nombre,
                Fecha = t.Fecha
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearTransaccionDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();

            var categoriaExiste = await _transaccionRepository.CategoriaExists(dto.CategoriaId);
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

            await _transaccionRepository.Create(transaccion);

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
            var transaccion = await _transaccionRepository.GetByIdAndUsuario(id, usuarioId);

            if (transaccion == null)
                return NotFound(new { mensaje = "Transacción no encontrada" });

            var categoriaExiste = await _transaccionRepository.CategoriaExists(dto.CategoriaId);
            if (!categoriaExiste)
                return BadRequest(new { mensaje = "La categoría no existe" });

            transaccion.Descripcion = dto.Descripcion;
            transaccion.Monto = dto.Monto;
            transaccion.Tipo = Enum.Parse<TipoTransaccion>(dto.Tipo);
            transaccion.CategoriaId = dto.CategoriaId;

            await _transaccionRepository.Update(transaccion);

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
            var transaccion = await _transaccionRepository.GetByIdAndUsuario(id, usuarioId);

            if (transaccion == null)
                return NotFound(new { mensaje = "Transacción no encontrada" });

            await _transaccionRepository.Delete(transaccion);

            return Ok(new { mensaje = "Transacción eliminada correctamente" });
        }

        // GET api/transaccion/excedente-mes
        [HttpGet("excedente-mes")]
        public async Task<IActionResult> GetExcedenteMes()
        {
            var usuarioId = ObtenerUsuarioId();
            var ahora = DateTime.UtcNow;
            var inicioMes = new DateTime(ahora.Year, ahora.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var transaccionesMes = await _transaccionRepository.GetByUsuarioIdDesde(usuarioId, inicioMes);

            decimal totalIngresos = transaccionesMes
                .Where(t => t.Tipo == TipoTransaccion.Ingreso)
                .Sum(t => t.Monto);

            decimal totalGastos = transaccionesMes
                .Where(t => t.Tipo == TipoTransaccion.Gasto)
                .Sum(t => t.Monto);

            decimal excedente = totalIngresos - totalGastos;

            return Ok(new
            {
                mes = ahora.ToString("MMMM yyyy"),
                totalIngresos,
                totalGastos,
                excedente = excedente > 0 ? excedente : 0
            });
        }
    }
}
