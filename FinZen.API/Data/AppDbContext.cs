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
        public DbSet<Meta> Metas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Transacciones)
                .HasForeignKey(t => t.UsuarioId);

            modelBuilder.Entity<Transaccion>()
                .Property(t => t.Monto)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Categoria)
                .WithMany(c => c.Transacciones)
                .HasForeignKey(t => t.CategoriaId);

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

            // Seed de categorías por defecto
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Comida", Tipo = "Gasto" },
                new Categoria { Id = 2, Nombre = "Transporte", Tipo = "Gasto" },
                new Categoria { Id = 3, Nombre = "Entretenimiento", Tipo = "Gasto" },
                new Categoria { Id = 4, Nombre = "Salud", Tipo = "Gasto" },
                new Categoria { Id = 5, Nombre = "Educación", Tipo = "Gasto" },
                new Categoria { Id = 6, Nombre = "Servicios", Tipo = "Gasto" },
                new Categoria { Id = 7, Nombre = "Salario", Tipo = "Ingreso" },
                new Categoria { Id = 8, Nombre = "Freelance", Tipo = "Ingreso" },
                new Categoria { Id = 9, Nombre = "Inversión", Tipo = "Ingreso" },
                new Categoria { Id = 10, Nombre = "Otros", Tipo = "Ambos" }
            );
        }
    }
}