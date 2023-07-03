using IntegrationProject.Data;
using IntegrationProject.Models;
using Microsoft.EntityFrameworkCore;

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
               .ToListAsync();

                return Results.Ok(flightList);
            });

            builder.MapDelete("flights/{id}", async (ApplicationDbContext _context, int id) =>
            {
                if (id <= 0)
                    return Results.BadRequest();

                var flight = await _context.Flight.SingleOrDefaultAsync(x => x.Id == id);

                if (flight == null)
                    return Results.NotFound();

                _context.Flight.Remove(flight);
                await _context.SaveChangesAsync();
                return Results.NoContent();
            });



            return builder;
        }
    }
}
