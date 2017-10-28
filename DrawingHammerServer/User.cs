namespace DrawingHammerServer
{
    public class User
    {        
        public int Id { get; }
        public string Username { get; }
        public string PasswordHash { get; }
        public bool IsBanned { get; }

        public User(int id, string username, string passwordHash, bool isBanned)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            IsBanned = isBanned;
        }
    }
}