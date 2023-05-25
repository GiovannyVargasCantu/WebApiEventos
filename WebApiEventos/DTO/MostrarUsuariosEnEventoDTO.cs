namespace WebApiEventos.DTO
{
    public class MostrarUsuariosEnEventoDTO
    {
        public int Id { get; set; }

        public int OrganizadorID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Ubicacion { get; set; }
        public int CapacidadMaximaAsistentes { get; set; }
        public List<UsuarioEventoDTO> UsuarioEventos { get; set; }
    }
}
