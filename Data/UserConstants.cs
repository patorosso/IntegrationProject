namespace IntegrationProject.Data
{
    public class UserConstants
    {
        public static List<UserModel> Users = new()
            {
                    new UserModel(){ Username="admin",Password="admin1234",Role="Admin"}
            };
    }
}
