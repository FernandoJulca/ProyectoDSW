using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace ApiToolify.ChatHub
{
    public class ChatHub : Hub
    {
        // Método para enviar un mensaje al grupo correspondiente
        public async Task SendMessage(string message)
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var userName = Context.User?.Identity?.Name;

            string roomId = GetRoomIdBasedOnUser(userId, role);

            await Clients.Group(roomId).SendAsync("ReceiveMessage", userName, message);
        }
        // Se ejecuta automáticamente cuando un cliente se conecta al hub
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            string roomId = GetRoomIdBasedOnUser(userId, role);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }
        // Se ejecuta automáticamente cuando un cliente se desconecta del hub
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            string roomId = GetRoomIdBasedOnUser(userId, role);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
        // Método para generar el ID de grupo o sala de chat basado en la lógica de usuario
        private string GetRoomIdBasedOnUser(string userId, string role)
        {
            return $"chat-{userId}"; 
        }
        // Método que permite a un cliente unirse manualmente a un grupo específico
        public async Task JoinGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }
        // Método para enviar un mensaje a un grupo específico
        public async Task SendMessageToGroup(string groupId, string user, string message)
        {
            await Clients.Group(groupId).SendAsync("ReceiveMessage", groupId, user, message);
        }
        // Notifica al grupo que una venta ha sido entregada
        public async Task ConfirmarEntrega(string idVenta)
        {

            await Clients.Group(idVenta).SendAsync("EntregaConfirmada", idVenta, "Tu pedido ha sido confirmado y entregado.");
        }
        // Notifica al grupo que una venta fue aceptada por el repartidor
        public async Task AceptarVenta(string idVenta)
        {
            await Clients.Group(idVenta).SendAsync("VentaAceptada", idVenta, "El repartidor ha aceptado tu pedido.");
        }
        // Notifica al grupo que una venta fue cancelada
        public async Task CancelarVenta(string idVenta)
        {
            await Clients.Group(idVenta).SendAsync("VentaCancelada", idVenta, "Tu pedido ha sido cancelado.");
        }
    }
}
