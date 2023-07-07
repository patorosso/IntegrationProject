using IntegrationProject.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IntegrationProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthJWTController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthJWTController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize] //usa cookie para acceder
        [HttpPost]
        public ActionResult GetToken([FromBody] UserModel userModel)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var token = GenerateToken(userModel);
                return Ok(token);
            }

            return NotFound("user not found");
        }

        private string GenerateToken(UserModel user)
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
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }


    }


}
