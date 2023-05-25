using Microsoft.AspNetCore.Identity;

namespace WebApiEventos.DTO
{
    public class FeedbacksDTO
    {
        public int Id { get; set; }
        public string Comentario { get; set; }
        public int EventoId { get; set; }
        public string UsuarioId { get; set; }
    }
}
