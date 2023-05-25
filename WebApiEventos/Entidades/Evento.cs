using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiEventos.Validaciones;

namespace WebApiEventos.Entidades
{
    public class Evento
    {
        public int Id { get; set; }
        public int OrganizadorID { get; set; }
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
        public int CapacidadMaximaAsistentes { get; set;}

        public List<UsuarioEvento> UsuarioEvento { get; set; }
        public Evento()
        {
            Feedbacks = new List<Feedback>();
        }
        public List<Feedback> Feedbacks { get; set; }
    }
}
