using IntegrationProject.Models;

namespace IntegrationProject.FlightState;

public class FlightState
{
    public bool ShowingConfigureDialog => ConfiguringFlight is not null;
    public Flight? ConfiguringFlight { get; private set; }

    public void ShowConfigureFlightDialog(Flight flight)
    {
        ConfiguringFlight = new()
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            AirlineId = flight.AirlineId,
            Delayed = flight.Delayed,
            Takeoff = flight.Takeoff
        };
    }

    public void CancelConfigureFlightDialog()
    {
        ConfiguringFlight = null;
    }

    public void ConfirmConfigureFlightDialog()
    {
        ConfiguringFlight = null;
    }


}
