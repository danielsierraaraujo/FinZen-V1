using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FinZen.API.Data;
using FinZen.API.DTOs;
using FinZen.API.Models;
using FinZen.API.Services;
using FinZen.API.Interfaces;

namespace FinZen.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MetaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MetaController(AppDbContext context)
        {
            _context = context;
        }

        private int ObtenerUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }

        // GET api/meta
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarioId = ObtenerUsuarioId();

            var metas = await _context.Metas
                .Where(m => m.UsuarioId == usuarioId)
                .Select(m => new MetaResponseDTO
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Descripcion = m.Descripcion,
                    MontoObjetivo = m.MontoObjetivo,
                    MontoActual = m.MontoActual,
                    Prioridad = m.Prioridad,
                    FechaLimite = m.FechaLimite,
                    PorcentajeCompletado = m.PorcentajeCompletado,
                    DiasRestantes = m.DiasRestantes
                })
                .ToListAsync();

            return Ok(metas);
        }

        // POST api/meta
        [HttpPost]
        public async Task<IActionResult> Create(CrearMetaDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();

            var meta = new Meta
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                MontoObjetivo = dto.MontoObjetivo,
                MontoActual = 0,
                Prioridad = dto.Prioridad,
                FechaLimite = dto.FechaLimite.ToUniversalTime(),
                UsuarioId = usuarioId
            };

            _context.Metas.Add(meta);
            await _context.SaveChangesAsync();

            return Ok(new MetaResponseDTO
            {
                Id = meta.Id,
                Nombre = meta.Nombre,
                Descripcion = meta.Descripcion,
                MontoObjetivo = meta.MontoObjetivo,
                MontoActual = meta.MontoActual,
                Prioridad = meta.Prioridad,
                FechaLimite = meta.FechaLimite,
                PorcentajeCompletado = meta.PorcentajeCompletado,
                DiasRestantes = meta.DiasRestantes
            });
        }

        // PUT api/meta/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CrearMetaDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
                return NotFound(new { mensaje = "Meta no encontrada" });

            meta.Nombre = dto.Nombre;
            meta.Descripcion = dto.Descripcion;
            meta.MontoObjetivo = dto.MontoObjetivo;
            meta.Prioridad = dto.Prioridad;
            meta.FechaLimite = dto.FechaLimite;

            await _context.SaveChangesAsync();

            return Ok(new MetaResponseDTO
            {
                Id = meta.Id,
                Nombre = meta.Nombre,
                Descripcion = meta.Descripcion,
                MontoObjetivo = meta.MontoObjetivo,
                MontoActual = meta.MontoActual,
                Prioridad = meta.Prioridad,
                FechaLimite = meta.FechaLimite,
                PorcentajeCompletado = meta.PorcentajeCompletado,
                DiasRestantes = meta.DiasRestantes
            });
        }

        // DELETE api/meta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = ObtenerUsuarioId();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
                return NotFound(new { mensaje = "Meta no encontrada" });

            _context.Metas.Remove(meta);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Meta eliminada correctamente" });
        }

        // POST api/meta/asignar
        [HttpPost("asignar")]
        public async Task<IActionResult> Asignar(AsignarExcedenteDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();

            var metas = await _context.Metas
                .Where(m => m.UsuarioId == usuarioId)
                .ToListAsync();

            if (!metas.Any())
                return BadRequest(new { mensaje = "No tienes metas creadas" });

            IEstrategiaAsignacion estrategia = dto.Estrategia switch
            {
                "prioridad"   => new EstrategiaPorPrioridad(),
                "urgencia"    => new EstrategiaPorUrgencia(),
                "equilibrada" => new EstrategiaEquilibrada(),
                _             => new EstrategiaPorPrioridad()
            };

            var asignador = new AsignadorService(estrategia);
            var resultado = asignador.Asignar(metas, dto.Excedente);

            var asignaciones = new List<AsignacionMetaDTO>();

            foreach (var (metaId, monto) in resultado)
            {
                var meta = metas.First(m => m.Id == metaId);
                decimal porcentajeAntes = meta.PorcentajeCompletado;

                meta.MontoActual += monto;

                decimal porcentajeDespues = meta.PorcentajeCompletado;

                asignaciones.Add(new AsignacionMetaDTO
                {
                    MetaId = metaId,
                    NombreMeta = meta.Nombre,
                    MontoAsignado = monto,
                    PorcentajeCompletadoAntes = Math.Round(porcentajeAntes, 2),
                    PorcentajeCompletadoDespues = Math.Round(porcentajeDespues, 2)
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new ResultadoAsignacionDTO
            {
                EstrategiaUsada = asignador.EstrategiaActual,
                ExcedenteTotal = dto.Excedente,
                Asignaciones = asignaciones
            });
        }
    }
}