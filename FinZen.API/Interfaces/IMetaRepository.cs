using FinZen.API.Models;

namespace FinZen.API.Interfaces
{
    public interface IMetaRepository
    {
        Task<List<Meta>> GetByUsuarioId(int usuarioId);
        Task<Meta?> GetByIdAndUsuario(int id, int usuarioId);
        Task<Meta> Create(Meta meta);
        Task<Meta> Update(Meta meta);
        Task Delete(Meta meta);
        Task SaveChanges();
    }
}