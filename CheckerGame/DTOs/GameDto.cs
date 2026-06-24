namespace CheckerGame.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Board { get; set; } = string.Empty; 
        public int CurrentTurnPlayerId { get; set; }
        public bool IsFinished { get; set; }
    }
}
