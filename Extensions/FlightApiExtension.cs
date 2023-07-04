using IntegrationProject.Data;
using IntegrationProject.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IntegrationProject.Extensions
{
    public static class FlightApiExtension
    {
        public static IEndpointRouteBuilder MapFlightApi(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("flights", async (ApplicationDbContext _context) =>
            {
                List<Flight> flightList = await _context.Flight
                .Include(c => c.Airline)
                .OrderBy(flight => flight.Takeoff)
                .ToListAsync();

                return Results.Ok(flightList);
            });
            builder.MapPost("flights", async (ApplicationDbContext _context, Flight flight) =>
            {
                if (flight == null)
                    return Results.BadRequest(flight);

                // if (flight.Id == 0)
                //    return Results.StatusCode(StatusCodes.Status500InternalServerError);


                string[] parts = flight.FlightNumber.Split(' ');
                if (parts.Length != 2)
                {
                    return Results.BadRequest("Invalid flight number");
                }
                string airlineCode = parts[0];

                {
                    var airline = _context.Airline.FirstOrDefault(a => a.Iata == airlineCode);
                    if (airline != null)
                    {
                        flight.AirlineId = airline.Id;
                    }
                    else
                    {
                        airline = _context.Airline.FirstOrDefault(a => a.Icao == airlineCode);
                        if (airline != null)
                        {
                            flight.AirlineId = airline.Id;
                        }
                        else return Results.BadRequest("Invalid flight number");
                    }
                }

                //check id duplicado
                var dbId = await _context.Flight.SingleOrDefaultAsync(c => c.Id == flight.Id);
                if (dbId != null && flight.Id == dbId.Id)
                {
                    return Results.BadRequest("Ya existe este id, por favor pruebe otro.");
                }

                //check numeroDeVuelo duplicado
                var duplicatedFlightInDb = await _context.Flight.SingleOrDefaultAsync(c => c.FlightNumber == flight.FlightNumber); // busco en mi db el num de vuelo
                if (duplicatedFlightInDb != null && duplicatedFlightInDb.Id != flight.Id) // si es un match, el unico valor que admito es el mismo, porque quizas quiero el mismo numVuelo, pero cambio detalles.
                {
                    return Results.BadRequest("Ya existe este numero de vuelo, por favor pruebe otro.");
                }

                await _context.Flight.AddAsync(flight);
                await _context.SaveChangesAsync();

                string serializedFlight = JsonConvert.SerializeObject(flight);
                return Results.Created($"/flights/{flight.Id}", serializedFlight);
            });

            builder.MapDelete("flights/{id}", async (ApplicationDbContext _context, int id) =>
            {
                if (id <= 0)
                    Results.BadRequest();

                var flight = await _context.Flight.SingleOrDefaultAsync(x => x.Id == id);

                if (flight == null)
                    return Results.NotFound();

                _context.Flight.Remove(flight);
                await _context.SaveChangesAsync();
                return Results.NoContent();
            });

            builder.MapPut("flights/{id}", async (ApplicationDbContext _context, int id, Flight flight) =>
            {

                if (flight == null)
                    Results.NotFound();


                string[] parts = flight!.FlightNumber.Split(' ');

                var airline = _context.Airline.FirstOrDefault(a => a.Iata == parts[0]);
                if (airline != null)
                {
                    flight.AirlineId = airline.Id;
                }
                else
                {
                    airline = _context.Airline.FirstOrDefault(a => a.Icao == parts[0]);
                    if (airline != null)
                    {
                        flight.AirlineId = airline.Id;
                    }
                    else flight.AirlineId = 1; // aerolinea desconocida en mi db
                }


                var flightInDb = await _context.Flight.SingleOrDefaultAsync(c => c.Id == flight.Id);

                if (flightInDb == null)
                    Results.NotFound();

                flightInDb!.FlightNumber = flight.FlightNumber.ToUpper();
                flightInDb!.Delayed = flight.Delayed;
                flightInDb!.Takeoff = flight.Takeoff;
                flightInDb!.AirlineId = flight.AirlineId;

                await _context.SaveChangesAsync();
                return Results.Ok(flight);
            });


            return builder;
        }
    }
}
