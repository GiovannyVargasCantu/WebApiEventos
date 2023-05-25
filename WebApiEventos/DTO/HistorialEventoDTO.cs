namespace WebApiEventos.DTO
{
    public class HistorialEventoDTO
    {
        public int EventoId { get; set; }
        public string NombreEvento { get; set; }
        public string DescripcionEvento { get; set; }
        public DateTime FechaEvento { get; set; }
        public bool Asistio { get; set; }
    }
}
