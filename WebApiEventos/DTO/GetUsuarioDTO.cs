
using System.ComponentModel.DataAnnotations;

namespace WebApiEventos.DTO
{
    public class GetUsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string CorreoElectronico { get; set; }

    }
}
