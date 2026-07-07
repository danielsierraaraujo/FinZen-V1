using FinZen.API.Models;

namespace FinZen.API.Interfaces
{
    public interface ITransaccionRepository
    {
        Task<List<Transaccion>> GetByUsuarioId(int usuarioId);
        Task<List<Transaccion>> GetByUsuarioIdDesde(int usuarioId, DateTime desde);
        Task<Transaccion?> GetByIdAndUsuario(int id, int usuarioId);
        Task<bool> CategoriaExists(int categoriaId);
        Task<Transaccion> Create(Transaccion transaccion);
        Task<Transaccion> Update(Transaccion transaccion);
        Task Delete(Transaccion transaccion);
    }
}
