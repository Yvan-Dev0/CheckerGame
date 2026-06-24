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
    }
}
