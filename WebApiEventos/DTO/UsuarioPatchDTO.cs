using System.ComponentModel.DataAnnotations;

namespace WebApiEventos.DTO
{
    public class UsuarioPatchDTO
    {
        [Required]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} solo puede tener hasta 40 caracteres")]
        public string Nombre { get; set; }
        [Required]
        [EmailAddress]
        public string CorreoElectronico { get; set; }
    }
}