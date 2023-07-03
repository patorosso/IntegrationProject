using System.ComponentModel.DataAnnotations;

namespace IntegrationProject.Models
{
    public class Airline
    {
        public short Id { get; set; }

        [StringLength(maximumLength: 100)]
        public string Name { get; set; } = null!;

        [StringLength(maximumLength: 40)]
        public string? Iata { get; set; }

        [StringLength(maximumLength: 15)]
        public string? Icao { get; set; }


    }
}
