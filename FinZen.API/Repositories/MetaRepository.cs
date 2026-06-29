using Microsoft.EntityFrameworkCore;
using FinZen.API.Data;
using FinZen.API.Interfaces;
using FinZen.API.Models;

namespace FinZen.API.Repositories
{
    public class MetaRepository : IMetaRepository
    {
        private readonly AppDbContext _context;

        public MetaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Meta>> GetByUsuarioId(int usuarioId)
        {
            return await _context.Metas
                .Where(m => m.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Meta?> GetByIdAndUsuario(int id, int usuarioId)
        {
            return await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);
        }

        public async Task<Meta> Create(Meta meta)
        {
            _context.Metas.Add(meta);
            await SaveChanges();
            return meta;
        }

        public async Task<Meta> Update(Meta meta)
        {
            _context.Metas.Update(meta);
            await SaveChanges();
            return meta;
        }

        public async Task Delete(Meta meta)
        {
            _context.Metas.Remove(meta);
            await SaveChanges();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}