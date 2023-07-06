using IntegrationProject.Authentication;
using IntegrationProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IntegrationProject.Controllers.Api
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public LoginController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        /*
        [HttpPost]
        public ActionResult GetTokenAPI([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);
            if (user != null)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier,user.Username!),
                new Claim(ClaimTypes.Role,user.Role!)
            };
                var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: credentials);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));

            }

            return NotFound("user not found");
        }
        */

        [Authorize(AuthenticationSchemes = "DefaultScheme")]
        [HttpPost]
        public ActionResult GetToken()
        {
            var currentUser = HttpContext.User;

            var usernameClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = currentUser.FindFirst(ClaimTypes.Role);

            var username = usernameClaim?.Value;
            var role = roleClaim?.Value;

            if (username == null || role == null)
                return BadRequest();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,username),
                new Claim(ClaimTypes.Role,role)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(token);
        }



        //To authenticate user
        private UserModel? Authenticate(UserLogin userLogin)
        {
            var currentUser = _context.CustomUser.FirstOrDefault(x => x.Username!.ToLower()! ==
                userLogin!.Username!.ToLower() && x.Password == userLogin!.Password);
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
    }


}
