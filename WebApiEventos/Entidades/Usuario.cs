using System.ComponentModel.DataAnnotations;
using WebApiEventos.Validaciones;

namespace WebApiEventos.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} solo puede tener hasta 40 caracteres")]

        public string Nombre { get; set; }
        [Required]
        [EmailAddress]
        public string CorreoElectronico { get; set; }

        public List<UsuarioEvento> UsuarioEvento { get; set; }

        public List<Evento> EventosFavoritos { get; set; }
        public List<CodigoPromocional> CodigosPromocionales { get; set; }
        public Usuario()
        {
            EventosFavoritos = new List<Evento>();
            CodigosPromocionales = new List<CodigoPromocional>();
        }
    }
}
