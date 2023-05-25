using WebApiEventos.Validaciones;
using System.ComponentModel.DataAnnotations;
using WebApiEventos.Entidades;

namespace WebApiEventos.DTO
{
    public class EventoCreacionDTO
    {

        public int OrganizadorID { get; set; }
        [Required]
        [StringLength(maximumLength: 40, ErrorMessage = "El campo {0} solo puede tener hasta 40 caracteres")]
        public string Nombre { get; set; }

        [StringLength(maximumLength: 150, ErrorMessage = "El campo {0} solo puede tener hasta 150 caracteres")]
        public string Descripcion { get; set; }
        [Required]
        public DateTime Fecha { get; set; }

        public string Ubicacion { get; set; }
        [Required]
        public int CapacidadMaximaAsistentes { get; set; }
        public List<int> UsuariosIDs { get; set; }
    }
}
