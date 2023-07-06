using IntegrationProject.Data.Models;
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
                       Username = ur.user.UserName,
                       Role = role.Name
                   }
               )
               .ToListAsync();

                return Results.Ok(userList);
            });

            return builder;
        }
    }
}
