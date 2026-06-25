namespace CheckerGame.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
    }
}
