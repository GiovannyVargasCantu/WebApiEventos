
namespace WebApiEventos.DTO
{
    public class UsuariosDTOConEventos : GetUsuarioDTO
    {
        public List<EventoDTO> Eventos { get; set; }

    }
}
