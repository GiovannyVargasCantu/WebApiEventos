using System.ComponentModel.DataAnnotations;
using WebApiEventos.Entidades;

namespace WebApiEventos.Entidades
{
    public class UsuarioEvento
    {
    
        public int UsuarioId { get; set; }

        public int EventoId { get; set; }
         public DateTime FechaRegistro { get; set; }
    public bool Asistio { get; set; }
        public int Orden {get; set; }

        public Evento Evento { get; set; }
        public Usuario Usuario { get; set; }
    }
}
