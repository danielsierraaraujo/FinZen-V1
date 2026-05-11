namespace FinZen.API.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public List<Transaccion> Transacciones { get; set; } = new();
    }
}