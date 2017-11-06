namespace DrawingHammerPacketLibrary
{
    public class Player
    {
        public int Id;
        public string Username;
        public string Uid;
        public string Score;

        public Player(int id, string username, string uid, string score)
        {
            Id = id;                  
            Username = username;
            Uid = uid;
            Score = score;
        }
    }
}