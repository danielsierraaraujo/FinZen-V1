namespace FinZen.API.DTOs
{
    public class CrearMetaDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal MontoObjetivo { get; set; }
        public int Prioridad { get; set; }
        public DateTime FechaLimite { get; set; }
    }

    public class MetaResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal MontoObjetivo { get; set; }
        public decimal MontoActual { get; set; }
        public int Prioridad { get; set; }
        public DateTime FechaLimite { get; set; }
        public decimal PorcentajeCompletado { get; set; }
        public int DiasRestantes { get; set; }
    }

    public class AsignarExcedenteDTO
    {
        public decimal Excedente { get; set; }
        public string Estrategia { get; set; } = string.Empty;
    }

    public class ResultadoAsignacionDTO
    {
        public string EstrategiaUsada { get; set; } = string.Empty;
        public decimal ExcedenteTotal { get; set; }
        public List<AsignacionMetaDTO> Asignaciones { get; set; } = new();
    }

    public class AsignacionMetaDTO
    {
        public int MetaId { get; set; }
        public string NombreMeta { get; set; } = string.Empty;
        public decimal MontoAsignado { get; set; }
        public decimal PorcentajeCompletadoAntes { get; set; }
        public decimal PorcentajeCompletadoDespues { get; set; }
    }

public class TopUsuarioDTO
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int MetasCompletadas { get; set; }
    public double PromedioDiasAdelantado { get; set; } 
}
}