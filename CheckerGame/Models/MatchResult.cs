namespace CheckerGame.Models
{
    public class MatchResult
    {
        public bool Waiting { get; set; }
        public WaitingPlayer? Player1 { get; set; }
        public WaitingPlayer? Player2 { get; set; }
    }
}
