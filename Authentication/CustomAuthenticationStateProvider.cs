using IntegrationProject.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace IntegrationProject.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ApplicationDbContext _context;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private IHttpContextAccessor _accessor;

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage,
            IHttpContextAccessor accessor, ApplicationDbContext context)
        {
            _sessionStorage = sessionStorage;
            _accessor = accessor;
            _context = context;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
            if (userSession == null)
                return await Task.FromResult(new AuthenticationState(_anonymous));

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, userSession.Username!),
                new Claim(ClaimTypes.Role, userSession.Role!)
            }, "CustomAuth"));
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task UpdateAuthenticationState(UserSession? userSession)
        {
            ClaimsIdentity claimsIdentity;


            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession!.Username!),
                    new Claim(ClaimTypes.Role, userSession!.Role!),
                };

            claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);


            var authProperties = new AuthenticationProperties
            { };

            await _accessor.HttpContext!.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
               new ClaimsPrincipal(claimsIdentity),
               authProperties);



            //NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(claimsIdentity))));
        }

        public async Task<string> UserLoginAsync(UserLogin userLogin)
        {

            UserModel? user = await _context.CustomUser
            .Where(c => c.Username!.ToLower() == userLogin.Username!.ToLower())
            .FirstOrDefaultAsync();

            if (user == null)
            {
                return "Invalid Credentials";
            }

            if (!ValidatePasswordHash(userLogin.Password!, user.PasswordHash!))
            {
                return "Invalid Credentials";
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Username!));
            claims.Add(new Claim(ClaimTypes.Role, user.Role!));


            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            { };

            await _accessor.HttpContext!.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
               new ClaimsPrincipal(claimsIdentity),
               authProperties);

            return string.Empty;
        }

        private bool ValidatePasswordHash(string password, string dbPassword)
        {
            byte[] dbPasswordHashBytes = Convert.FromBase64String(dbPassword);

            byte[] salt = new byte[16];
            Array.Copy(dbPasswordHashBytes, 0, salt, 0, 16);

            var userPasswordBytes = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] userPasswordHash = userPasswordBytes.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (dbPasswordHashBytes[i + 16] != userPasswordHash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
