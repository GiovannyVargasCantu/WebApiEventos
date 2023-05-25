namespace WebApiEventos.DTO
{
    public class UsuarioEventoDTO
    {
        public int UsuarioId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Asistio { get; set; }
        public int EventoId { get; set; }
        public int Orden { get; set; }


    }
}
