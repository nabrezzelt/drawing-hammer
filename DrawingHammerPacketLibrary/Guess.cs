namespace DrawingHammerPacketLibrary
{
    public class Guess : ViewModelBase
    {
        public string PlayerName { get; set; }

        public string GuessMessage { get; set; }

        public bool IsCorrect { get; set; }

        public Guess(string playerName, string guessMessage, bool isCorrect)
        {
            PlayerName = playerName;
            GuessMessage = guessMessage;
            IsCorrect = isCorrect;
        }
    }
}
