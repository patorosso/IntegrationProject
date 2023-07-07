using IntegrationProject.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IntegrationProject.Extensions
{
    public static class UserApiExtension
    {
        public static IEndpointRouteBuilder MapUserApi(this IEndpointRouteBuilder builder)
        {

            builder.MapGet("users", async (ApplicationDbContext _context) =>
            {
                List<UserModel> userList = await _context.Users
               .Join(
                   _context.UserRoles,
                   user => user.Id,
                   userRole => userRole.UserId,
                   (user, userRole) => new { user, userRole }
               )
               .Join(
                   _context.Roles,
                   ur => ur.userRole.RoleId,
                   role => role.Id,
                   (ur, role) => new UserModel
                   {
                       Id = ur.user.Id,
                       Username = ur.user.UserName,
                       Role = role.Name
                   }
               )
               .ToListAsync();

                return Results.Ok(userList);
            });

            builder.MapPut("users/{id}", async (ApplicationDbContext _context, UserManager<IdentityUser> _userManager,
                string id, UserModel userModel) =>
            {
                if (userModel == null)
                    return Results.BadRequest();

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return Results.NotFound();

                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, userModel.Role!);

                return Results.Ok(userModel);
            }).RequireAuthorization("JWTScheme");

            builder.MapDelete("users/{id}", async (ApplicationDbContext _context, UserManager<IdentityUser> _userManager,
                string id) =>
            {
                if (id == null)
                    return Results.BadRequest();
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return Results.NotFound();

                await _userManager.DeleteAsync(user);

                return Results.Ok();

            }).RequireAuthorization("JWTScheme");

            return builder;
        }
    }
}
