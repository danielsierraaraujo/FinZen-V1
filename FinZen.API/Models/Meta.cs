namespace FinZen.API.Models
{
    public class Meta
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal MontoObjetivo { get; set; }
        public decimal MontoActual { get; set; }
        public int Prioridad { get; set; }
        public DateTime FechaLimite { get; set; }
        public DateTime? FechaCompletada { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public decimal PorcentajeCompletado =>
            MontoObjetivo > 0 ? (MontoActual / MontoObjetivo) * 100 : 0;

        public int DiasRestantes =>
            (FechaLimite - DateTime.UtcNow).Days;


        public int? DiasAdelantado =>
            FechaCompletada.HasValue
                ? (FechaLimite - FechaCompletada.Value).Days
                : null;
    }
}