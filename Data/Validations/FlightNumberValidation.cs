using IntegrationProject.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IntegrationProject.Data.Validations
{
    public class FlightNumberValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            var flight = (Flight)validationContext.ObjectInstance;

            if (flight.FlightNumber == null || flight.FlightNumber.Length == 0)
            {
                return new ValidationResult("The field is required.");
            }

            if (Regex.IsMatch(flight.FlightNumber, @"^[a-zA-Z]+$"))
            {
                return new ValidationResult("There must be numbers as well.");
            }

            if (Regex.IsMatch(flight.FlightNumber, "^[0-9]+$"))
            {
                return new ValidationResult("There must be letters as well.");
            }

            string[] parts = flight.FlightNumber.Split(' ');
            if (parts.Length != 2)
            {
                return new ValidationResult("There must be a space between airline code and number.");
            }
            string airlineCodeLetters = parts[0]; // primera parte

            if (!Regex.IsMatch(airlineCodeLetters, @"^[a-zA-Z]+$"))
            {
                return new ValidationResult("The first part must be letters.");
            }

            string airlineCodeNumbers = parts[1]; // segunda parte

            if (!Regex.IsMatch(airlineCodeNumbers, "^[0-9]+$"))
            {
                return new ValidationResult("The second part must contain only numbers.");
            }

            return ValidationResult.Success;

        }
    }
}

