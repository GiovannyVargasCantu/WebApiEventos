namespace WebApiEventos.Entidades
{
    public class CodigoPromocional
    {
        public int Id { get; set; }
        public string Codigo { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

    }
}
