using IntegrationProject.Authentication;
using IntegrationProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntegrationProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Flight> Flight => Set<Flight>();
        public DbSet<Airline> Airline => Set<Airline>();

        public DbSet<UserModel> CustomUser => Set<UserModel>();
    }
}