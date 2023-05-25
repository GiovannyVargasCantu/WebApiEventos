using Microsoft.AspNetCore.Identity;

namespace WebApiEventos.Entidades
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Comentario { get; set; }

        public int EventoId { get; set; }
        public Evento Evento { get; set; }

        public string UsuarioId { get; set; }
    }
}
