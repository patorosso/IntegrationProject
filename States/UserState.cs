using IntegrationProject.Data.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace IntegrationProject.States
{
    public class UserState
    {
        public bool ShowingEditDialog;
        public bool ShowingDeleteDialog;

        public DateTime TokenLife;
        private string? Token;

        public UserModel? UserModel { get; set; }
        public event Func<Task>? OnConfirmConfigureUserDialog;

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _accessor;
        private readonly NavigationManager _navigationManager;

        public UserState(HttpClient httpClient, NavigationManager navigationManager, IHttpContextAccessor accessor)
        {
            this._httpClient = httpClient;
            this._navigationManager = navigationManager;
            this._accessor = accessor;
        }

        public void ShowConfigureUserDialog(UserModel user, String type)
        {
            UserModel = new()
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            if (type == "Edit")
                ShowingEditDialog = true;
            else ShowingDeleteDialog = true;
        }


        public void CancelConfigureUserDialog()
        {
            UserModel = null;
            ShowingEditDialog = false;
            ShowingDeleteDialog = false;
        }

        public async Task ConfirmEditUserDialog()
        {
            TimeSpan diff = TokenLife - DateTime.Now;

            if (Token == null || diff <= TimeSpan.Zero)
                await GetTokenUsingCookie();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var content = new StringContent(JsonConvert.SerializeObject(UserModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_navigationManager.BaseUri}users/{UserModel?.Id}", content);

            UserModel = null;
            OnConfirmConfigureUserDialog?.Invoke();
            ShowingEditDialog = false;
        }

        public async Task ConfirmDeleteUserDialog()
        {
            TimeSpan diff = TokenLife - DateTime.Now;

            if (Token == null || diff <= TimeSpan.Zero)
                await GetTokenUsingCookie();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var response = await _httpClient.DeleteAsync($"{_navigationManager.BaseUri}users/{UserModel?.Id}");

            OnConfirmConfigureUserDialog?.Invoke();
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

            _httpClient.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Identity.Application={cookieValue}"); //agrego la cookie al header de la request


            var content = new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_navigationManager.BaseUri}api/AuthJWT/", content);

            Token = await response.Content.ReadAsStringAsync();
            TokenLife = DateTime.Now.AddMinutes(2);

        }

    }
}
