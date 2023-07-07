using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationProject.Controllers
{

    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> SeedRoles()
        {
            bool roleExists = await _roleManager.RoleExistsAsync("Admin");
            roleExists = await _roleManager.RoleExistsAsync("User");
            if (roleExists)
            {
                return Ok("Los roles ya estaban cargados.");
            }

            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("User"));

            return Ok("Los roles fueron correctamente cargados.");
        }
    }
}

