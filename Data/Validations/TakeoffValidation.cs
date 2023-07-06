using IntegrationProject.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace IntegrationProject.Data.Validations
{
    public class TakeoffValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            var flight = (Flight)validationContext.ObjectInstance;

            if (flight.Takeoff <= DateTime.Now)
            {
                return new ValidationResult("Takeoff time must be ahead of the current time.");
            }

            return ValidationResult.Success;
        }
    }

}
