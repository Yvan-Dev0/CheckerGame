namespace CheckerGame.DTOs
{
    public class PLayerDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}
