using Microsoft.AspNetCore.SignalR;

namespace CheckerGame.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendMove(string gameId, string move)
        {
            await Clients.Group(gameId).SendAsync("ReceiveMove", move);
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).SendAsync("PlayerJoined", Context.ConnectionId);
        }

        public async Task MakeMove(string gameId, int fromX, int fromY, int toX, int toY)
        {
            await Clients.Group(gameId).SendAsync("GameUpdated", new
            {
                fromX,
                fromY,
                toX,
                toY
            });
        }
    }
}
