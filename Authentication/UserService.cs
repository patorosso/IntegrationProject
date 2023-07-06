using IntegrationProject.Data;

namespace IntegrationProject.Authentication
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserModel? GetByUserName(string username)
        {
            return _context.CustomUser.FirstOrDefault(x => x.Username == username);
        }
    }
}
