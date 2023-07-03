using System.ComponentModel.DataAnnotations;

namespace IntegrationProject.Dtos
{
    public class FlightDto
    {
        public byte Id { get; set; }

        [Required]
        public string FlightNumber { get; set; } = null!;


        public short AirlineId { get; set; }


        public AirlineDto? Aerolinea { get; set; }


        public bool Delayed { get; set; }

        [Required]
        public DateTime? Takeoff { get; set; }
    }
}
