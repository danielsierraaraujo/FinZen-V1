namespace FinZen.API.Models
{
    public enum TipoTransaccion
    {
        Ingreso,
        Gasto
    }

    public class Transaccion
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public TipoTransaccion Tipo { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}