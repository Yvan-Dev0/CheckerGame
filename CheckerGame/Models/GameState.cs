namespace CheckerGame.Models
{
    public class GameState
    {
        public int Id { get; set; }
        public string Board { get; set; } = string.Empty; //JSON representation of board
        public int CurrentTurnPlayerId { get; set; }
        public bool IsFinished { get; set; }
    }
}
