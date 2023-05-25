using Microsoft.EntityFrameworkCore;
using WebApiEventos.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebApiEventos
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioEvento>()
                .HasKey(al => new { al.UsuarioId, al.EventoId });
        }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<UsuarioEvento> UsuarioEvento { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<CodigoPromocional> CodigosPromocionales { get; set; }

    }
}