using System.ComponentModel.DataAnnotations;

namespace WebApiEventos.Validaciones
{
    public class ValidacionFechaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            DateTime fecha;
            if (!DateTime.TryParse(value.ToString(), out fecha))
            {
                return new ValidationResult("El valor proporcionado no es una fecha válida.");
            }

            if (fecha.Date < DateTime.Now.Date)
            {
                return new ValidationResult("La fecha no puede ser anterior a la fecha actual.");
            }

            return ValidationResult.Success;
        }
    }
}
