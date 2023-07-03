using System.ComponentModel.DataAnnotations;

namespace IntegrationProject.Models
{
    public class Flight
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio.")]
        [FlightNumberValidation]
        [StringLength(maximumLength: 12)]
        public string FlightNumber { get; set; } = null!;

        public Airline? Airline { get; set; }


        public short AirlineId { get; set; }


        public bool Delayed { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]
        public DateTime? Takeoff { get; set; }





    }
}
