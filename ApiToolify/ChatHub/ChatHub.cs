using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace ApiToolify.ChatHub
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<int, List<string>> _connections 
            = new ConcurrentDictionary<int, List<string>>();

        public Task RegisterUser(int userId) { 
           var connectionId = Context.ConnectionId;
            var list = _connections.GetOrAdd(userId, _ => new List<string>());
           
            lock (list)
            {
                if (!list.Contains(connectionId))
                    list.Add(connectionId);
            }
            return Task.CompletedTask;
        }

        public async Task SendMessage(int userIdDestino, string mensaje) {
            System.Diagnostics.Debug.WriteLine("************************** INICIANDO EL METODO SENDMESAGGE **************************");
            System.Diagnostics.Debug.WriteLine($"Llega userIdDestino={userIdDestino}, mensaje={mensaje}");
            if (_connections.TryGetValue(userIdDestino, out var connectionIds)){
                foreach (var connection in connectionIds) {
                    await Clients.Client(connection).SendAsync("ReceiveMessage", mensaje);
                }
            }
        }
    }
}
