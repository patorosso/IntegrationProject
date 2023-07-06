using IntegrationProject.Data.Validations;
using System.ComponentModel.DataAnnotations;

namespace IntegrationProject.Data.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field is required.")]
        [FlightNumberValidation]
        [StringLength(maximumLength: 12)]
        public string FlightNumber { get; set; } = null!;

        public Airline? Airline { get; set; }

        public short AirlineId { get; set; }

        public bool Delayed { get; set; }

        [TakeoffValidation]
        [Required(ErrorMessage = "The field is required.")]
        public DateTime? Takeoff { get; set; }


    }
}
