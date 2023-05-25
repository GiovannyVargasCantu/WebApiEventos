using System.ComponentModel.DataAnnotations;
using WebApiEventos.Validaciones;

namespace WebApiEventos.DTO
{
    public class EventoPatchDTO
    {
        [Required]
        [StringLength(maximumLength: 40, ErrorMessage = "El campo {0} solo puede tener hasta 40 caracteres")]
        public string Nombre { get; set; }
        [StringLength(maximumLength: 150, ErrorMessage = "El campo {0} solo puede tener hasta 150 caracteres")]
        public string Descripcion { get; set; }
        [Required]
        [ValidacionFechaAttribute]
        public DateTime Fecha { get; set; }

        public string Ubicacion { get; set; }
        [Required]
        public int CapacidadMaximaAsistentes { get; set; }
    }
}
