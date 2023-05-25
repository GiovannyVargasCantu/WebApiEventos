using System.ComponentModel.DataAnnotations;
using WebApiEventos.Entidades;

namespace WebApiEventos.DTO
{
    public class EventosDestacadosDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Ubicacion { get; set; }
        public int CapacidadMaximaAsistentes { get; set; }
    }
}
