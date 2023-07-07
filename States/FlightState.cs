using IntegrationProject.Data.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace IntegrationProject.States;

public class FlightState
{
    public bool ShowingPostDialog;
    public bool ShowingEditDialog;
    public bool ShowingDeleteDialog;

    public DateTime TokenLife;
    private string? Token;
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
        TimeSpan diff = TokenLife - DateTime.Now;

        if (Token == null || diff <= TimeSpan.Zero)
            await GetTokenUsingCookie();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        var content = new StringContent(JsonConvert.SerializeObject(ConfiguringFlight), Encoding.UTF8, "application/json");

        var response = await httpClient.PutAsync($"{navigationManager.BaseUri}flights/{ConfiguringFlight?.Id}", content);

        ConfiguringFlight = null;
        OnConfirmConfigureFlightDialog?.Invoke();
        ShowingEditDialog = false;
    }

    public async Task ConfirmPostFlightDialog()
    {
        TimeSpan diff = TokenLife - DateTime.Now;

        if (Token == null || diff <= TimeSpan.Zero)
            await GetTokenUsingCookie();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        var content = new StringContent(JsonConvert.SerializeObject(ConfiguringFlight), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{navigationManager.BaseUri}flights/", content);

        ConfiguringFlight = null;
        OnConfirmConfigureFlightDialog?.Invoke();
        ShowingPostDialog = false;
    }

    public async Task ConfirmDeleteFlightDialog()
    {
        TimeSpan diff = TokenLife - DateTime.Now;

        if (Token == null || diff <= TimeSpan.Zero)
            await GetTokenUsingCookie();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        var response = await httpClient.DeleteAsync($"{navigationManager.BaseUri}flights/{ConfiguringFlight?.Id}");

        OnConfirmConfigureFlightDialog?.Invoke();
        ShowingDeleteDialog = false;
    }

    private async Task GetTokenUsingCookie()
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

        string cookieValue = _accessor.HttpContext!.Request.Cookies[".AspNetCore.Identity.Application"]!;

        httpClient.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Identity.Application={cookieValue}"); //agrego la cookie al header de la request


        var content = new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{navigationManager.BaseUri}api/AuthJWT/", content);

        Token = await response.Content.ReadAsStringAsync();
        TokenLife = DateTime.Now.AddMinutes(2);

    }


}
