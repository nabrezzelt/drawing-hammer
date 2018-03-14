namespace DrawingHammerServer
{
    public class User
    {        
        public int Id { get; }
        public string Username { get; }
        public string PasswordHash { get; }        

        public User(int id, string username, string passwordHash)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;            
        }
    }
}