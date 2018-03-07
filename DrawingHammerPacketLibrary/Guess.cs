namespace DrawingHammerPacketLibrary
{
    public class Guess : ViewModelBase
    {
        public string PlayerName { get; }

        public string GuessMessage { get; }

        public bool IsCorrect { get; }

        public Guess(string playerName, string guessMessage, bool isCorrect)
        {
            PlayerName = playerName;
            GuessMessage = guessMessage;
            IsCorrect = isCorrect;
        }
    }
}
