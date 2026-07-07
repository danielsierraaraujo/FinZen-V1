using Microsoft.EntityFrameworkCore;
using FinZen.API.Data;
using FinZen.API.Interfaces;
using FinZen.API.Models;

namespace FinZen.API.Repositories
{
    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly AppDbContext _context;

        public TransaccionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaccion>> GetByUsuarioId(int usuarioId)
        {
            return await _context.Transacciones
                .Include(t => t.Categoria)
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<List<Transaccion>> GetByUsuarioIdDesde(int usuarioId, DateTime desde)
        {
            return await _context.Transacciones
                .Where(t => t.UsuarioId == usuarioId && t.Fecha >= desde)
                .ToListAsync();
        }

        public async Task<Transaccion?> GetByIdAndUsuario(int id, int usuarioId)
        {
            return await _context.Transacciones
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);
        }

        public async Task<bool> CategoriaExists(int categoriaId)
        {
            return await _context.Categorias.AnyAsync(c => c.Id == categoriaId);
        }

        public async Task<Transaccion> Create(Transaccion transaccion)
        {
            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();
            await _context.Entry(transaccion).Reference(t => t.Categoria).LoadAsync();
            return transaccion;
        }

        public async Task<Transaccion> Update(Transaccion transaccion)
        {
            _context.Transacciones.Update(transaccion);
            await _context.SaveChangesAsync();
            return transaccion;
        }

        public async Task Delete(Transaccion transaccion)
        {
            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();
        }
    }
}
