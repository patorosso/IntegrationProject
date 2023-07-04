using IntegrationProject.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Text;

namespace IntegrationProject.FlightState;

public class FlightState
{
    public bool ShowingPostDialog;
    public bool ShowingEditDialog;
    public bool ShowingDeleteDialog;

    public Flight? ConfiguringFlight { get; private set; }

    public event Func<Task>? OnConfirmConfigureFlightDialog;

    private readonly HttpClient httpClient;
    private readonly NavigationManager navigationManager;

    public FlightState(HttpClient httpClient, NavigationManager navigationManager)
    {
        this.httpClient = httpClient;
        this.navigationManager = navigationManager;
    }

    public void ShowConfigureFlightDialog(Flight flight, String type)
    {
        ConfiguringFlight = new()
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            AirlineId = flight.AirlineId,
            Delayed = flight.Delayed,
            Takeoff = flight.Takeoff
        };

        if (type == "Edit")
            ShowingEditDialog = true;
        else if (type == "Post")
            ShowingPostDialog = true;
        else ShowingDeleteDialog = true;
    }

    public void CancelConfigureFlightDialog()
    {
        ConfiguringFlight = null;
        ShowingEditDialog = false;
        ShowingPostDialog = false;
        ShowingDeleteDialog = false;
    }

    public async Task ConfirmEditFlightDialog()
    {
        var content = new StringContent(JsonConvert.SerializeObject(ConfiguringFlight), Encoding.UTF8, "application/json");

        var response = await httpClient.PutAsync($"{navigationManager.BaseUri}flights/{ConfiguringFlight?.Id}", content);

        ConfiguringFlight = null;
        OnConfirmConfigureFlightDialog?.Invoke();
        ShowingEditDialog = false;
    }

    public async Task ConfirmPostFlightDialog()
    {
        var content = new StringContent(JsonConvert.SerializeObject(ConfiguringFlight), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{navigationManager.BaseUri}flights/", content);

        ConfiguringFlight = null;
        OnConfirmConfigureFlightDialog?.Invoke();
        ShowingPostDialog = false;
    }

    public async Task ConfirmDeleteFlightDialog()
    {
        var response = await httpClient.DeleteAsync($"{navigationManager.BaseUri}flights/{ConfiguringFlight?.Id}");

        OnConfirmConfigureFlightDialog?.Invoke();
        ShowingDeleteDialog = false;
    }


}
