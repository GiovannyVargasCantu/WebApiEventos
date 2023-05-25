using System.ComponentModel.DataAnnotations;

namespace WebApiEventos.DTO
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}