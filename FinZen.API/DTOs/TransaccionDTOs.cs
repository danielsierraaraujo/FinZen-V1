namespace FinZen.API.DTOs
{
    public class CrearTransaccionDTO
    {
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
    }

    public class TransaccionResponseDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }

    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
}