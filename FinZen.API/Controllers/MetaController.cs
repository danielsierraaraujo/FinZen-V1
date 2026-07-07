using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinZen.API.DTOs;
using FinZen.API.Interfaces;
using FinZen.API.Models;
using FinZen.API.Services;

namespace FinZen.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MetaController : ControllerBase
    {
        private readonly IMetaRepository _metaRepository;
        private readonly IEstrategiaAsignacionFactory _estrategiaFactory;

        public MetaController(IMetaRepository metaRepository, IEstrategiaAsignacionFactory estrategiaFactory)
        {
            _metaRepository = metaRepository;
            _estrategiaFactory = estrategiaFactory;
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
            var metas = await _metaRepository.GetByUsuarioId(usuarioId);

            return Ok(metas.Select(m => new MetaResponseDTO
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
            }));
        }

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

            await _metaRepository.Create(meta);

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CrearMetaDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();
            var meta = await _metaRepository.GetByIdAndUsuario(id, usuarioId);

            if (meta == null)
                return NotFound(new { mensaje = "Meta no encontrada" });

            meta.Nombre = dto.Nombre;
            meta.Descripcion = dto.Descripcion;
            meta.MontoObjetivo = dto.MontoObjetivo;
            meta.Prioridad = dto.Prioridad;
            meta.FechaLimite = dto.FechaLimite.ToUniversalTime();

            await _metaRepository.Update(meta);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarioId = ObtenerUsuarioId();
            var meta = await _metaRepository.GetByIdAndUsuario(id, usuarioId);

            if (meta == null)
                return NotFound(new { mensaje = "Meta no encontrada" });

            await _metaRepository.Delete(meta);
            return Ok(new { mensaje = "Meta eliminada correctamente" });
        }

        [HttpPost("asignar")]
        public async Task<IActionResult> Asignar(AsignarExcedenteDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();
            var metas = await _metaRepository.GetByUsuarioId(usuarioId);

            if (!metas.Any())
                return BadRequest(new { mensaje = "No tienes metas creadas" });

            var estrategia = _estrategiaFactory.Crear(dto.Estrategia);
            var asignador = new AsignadorService(estrategia);
            var resultado = asignador.Asignar(metas, dto.Excedente);

            var asignaciones = new List<AsignacionMetaDTO>();

            foreach (var (metaId, monto) in resultado)
            {
                var meta = metas.First(m => m.Id == metaId);
                decimal porcentajeAntes = meta.PorcentajeCompletado;

                meta.MontoActual += monto;

                if (meta.PorcentajeCompletado >= 100 && !meta.FechaCompletada.HasValue)
                    meta.FechaCompletada = DateTime.UtcNow;

                decimal porcentajeDespues = meta.PorcentajeCompletado;

                asignaciones.Add(new AsignacionMetaDTO
                {
                    MetaId = metaId,
                    NombreMeta = meta.Nombre,
                    MontoAsignado = monto,
                    PorcentajeCompletadoAntes = Math.Round(porcentajeAntes, 2),
                    PorcentajeCompletadoDespues = Math.Round(porcentajeDespues, 2)
                });

                await _metaRepository.Update(meta);
            }

            return Ok(new ResultadoAsignacionDTO
            {
                EstrategiaUsada = asignador.EstrategiaActual,
                ExcedenteTotal = dto.Excedente,
                Asignaciones = asignaciones
            });
        }
    }
}