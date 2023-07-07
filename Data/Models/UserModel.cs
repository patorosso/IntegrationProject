namespace IntegrationProject.Data.Models
{
    public class UserModel
    {
        public string Id { get; set; } = null!;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public bool hasToken { get; set; }
    }
}
