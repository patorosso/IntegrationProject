using IntegrationProject.Data;
using IntegrationProject.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
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
    private readonly IHttpContextAccessor _accessor;
    private readonly NavigationManager navigationManager;

    public FlightState(HttpClient httpClient, NavigationManager navigationManager, IHttpContextAccessor accessor)
    {
        this.httpClient = httpClient;
        this.navigationManager = navigationManager;
        this._accessor = accessor;
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

        string username = _accessor.HttpContext!.User.Identity!.Name!;

        IList<string> roles = _accessor.HttpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        UserModel userModel = new UserModel()
        {
            Username = username,
            Role = roles.FirstOrDefault()
        };

        var content = new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{navigationManager.BaseUri}api/Login/", content);
        string token = await response.Content.ReadAsStringAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        response = await httpClient.DeleteAsync($"{navigationManager.BaseUri}flights/{ConfiguringFlight?.Id}");

        OnConfirmConfigureFlightDialog?.Invoke();
        ShowingDeleteDialog = false;
    }


}
