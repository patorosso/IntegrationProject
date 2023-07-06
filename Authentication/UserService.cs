using IntegrationProject.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IntegrationProject.Authentication
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private IHttpContextAccessor _accessor;

        public UserService(ApplicationDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public UserModel? GetByUserName(string username)
        {
            return _context.CustomUser.FirstOrDefault(x => x.Username == username);
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
            /*
            if (!ValidatePasswordHash(userLogin.Password!, user.PasswordHash!))
            {
                return "Invalid Credentials";
            }
            */

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
        /*
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
        */
    }
}
