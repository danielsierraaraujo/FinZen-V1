using Microsoft.EntityFrameworkCore;
using FinZen.API.Models;

namespace FinZen.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<Meta> Metas { get; set; }  // ← agrega esta línea

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Transacciones)
                .HasForeignKey(t => t.UsuarioId);

            modelBuilder.Entity<Transaccion>()
                .Property(t => t.Monto)
                .HasPrecision(18, 2);

            // Relación Meta → Usuario
            modelBuilder.Entity<Meta>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId);

            modelBuilder.Entity<Meta>()
                .Property(m => m.MontoObjetivo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Meta>()
                .Property(m => m.MontoActual)
                .HasPrecision(18, 2);
        }
    }
}