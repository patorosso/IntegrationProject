namespace IntegrationProject.Controllers.Api
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class FlightsController // : ControllerBase
    {
    }
    //    private readonly ApplicationDbContext _context;
    //    private readonly IMapper _mapper;

    //    public FlightsController(ApplicationDbContext context, IMapper mapper)
    //    {
    //        _context = context;
    //        _mapper = mapper;
    //    }

    //    // GET /api/vuelos/
    //    [HttpGet]
    //    [ProducesResponseType(StatusCodes.Status200OK)]
    //    public async Task<List<Flight>> GetFlights()
    //    {
    //        List<Flight> flightList = await _context.Flight
    //            .Include(c => c.Airline)
    //            .ToListAsync();

    //        return flightList;
    //    }

    //    // GET /api/vuelos/id
    //    [HttpGet("{id:int}")]
    //    [ProducesResponseType(StatusCodes.Status200OK)]
    //    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    public async Task<ActionResult<FlightDto>> GetFlight(int id)
    //    {
    //        if (id == 0)
    //            return BadRequest();

    //        var flight = await _context.Flight
    //            .Include(c => c.Airline)
    //            .SingleOrDefaultAsync(x => x.Id == id);

    //        if (flight == null)
    //            return NotFound();

    //        return Ok(_mapper.Map<FlightDto>(flight));
    //    }

    //    // POST /api/vuelos
    //    [HttpPost]
    //    [ProducesResponseType(StatusCodes.Status201Created)]
    //    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //    public async Task<ActionResult<FlightDto>> PostFlight([FromBody] FlightDto flightDto)
    //    {
    //        if (!ModelState.IsValid)
    //            return BadRequest(ModelState);

    //        if (flightDto == null)
    //            return BadRequest(flightDto);

    //        if (flightDto.Id == 0)
    //            return StatusCode(StatusCodes.Status500InternalServerError);

    //        // Split the flight number into airline code and flight number
    //        string[] parts = flightDto.FlightNumber.Split(' ');
    //        if (parts.Length != 2)
    //        {
    //            // Handle invalid flight number
    //            return BadRequest("Invalid flight number");
    //        }
    //        string airlineCode = parts[0];

    //        {
    //            var airline = _context.Airline.FirstOrDefault(a => a.Iata == airlineCode);
    //            if (airline != null)
    //            {
    //                flightDto.AirlineId = airline.Id;
    //            }
    //            else
    //            {
    //                airline = _context.Airline.FirstOrDefault(a => a.Icao == airlineCode);
    //                if (airline != null)
    //                {
    //                    flightDto.AirlineId = airline.Id;
    //                }
    //                else return BadRequest("Invalid flight number");
    //            }
    //        }

    //        //check id duplicado
    //        var dbId = await _context.Flight.SingleOrDefaultAsync(c => c.Id == flightDto.Id);
    //        if (dbId != null && flightDto.Id == dbId.Id)
    //        {
    //            ModelState.AddModelError("Id existente", "Ya existe este id, por favor pruebe otro.");
    //            return BadRequest(ModelState);
    //        }
    //        //check numeroDeVuelo duplicado
    //        var DuplicatedFlightInDb = await _context.Flight.SingleOrDefaultAsync(c => c.FlightNumber == flightDto.FlightNumber); // busco en mi db el num de vuelo
    //        if (DuplicatedFlightInDb != null && DuplicatedFlightInDb.Id != flightDto.Id) // si es un match, el unico valor que admito es el mismo, porque quizas quiero el mismo numVuelo, pero cambio detalles.
    //        {
    //            ModelState.AddModelError("Numero de vuelo existente", "Ya existe este numero de vuelo, por favor pruebe otro.");
    //            return BadRequest(ModelState);
    //        }

    //        Flight postedFlight = _mapper.Map<Flight>(flightDto); // creo un modelo de Vuelo usando el vueloDto del param de la func

    //        await _context.Flight.AddAsync(postedFlight);
    //        await _context.SaveChangesAsync();

    //        return CreatedAtAction(nameof(GetFlight), new { id = flightDto.Id }, flightDto);
    //    }


    //    // DELETE /api/vuelos/id
    //    [HttpDelete("{id:int}")]
    //    [ProducesResponseType(StatusCodes.Status204NoContent)]
    //    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    public async Task<IActionResult> DeleteFlight(int? id)
    //    {

    //        if (id == 0 || id == null)
    //            return BadRequest();

    //        var flight = await _context.Flight.SingleOrDefaultAsync(x => x.Id == id);

    //        if (flight == null)
    //            return NotFound();

    //        _context.Flight.Remove(flight);
    //        await _context.SaveChangesAsync();
    //        return NoContent();
    //    }



}
