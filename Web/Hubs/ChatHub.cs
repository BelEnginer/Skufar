using Application.Abstractions.IHubs;
using Application.DTOs.GetDtos;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;

public class ChatHub : Hub, IChatHub
{

   public async Task SendMessage(string receiverId,MessageDto message,CancellationToken ct)
   {
      await Clients.User(receiverId)
         .SendAsync("ReceiveMessage", message,ct);
   }

   public async Task DeleteMessage(string receiverId,MessageDto message,CancellationToken ct)
   {
      await Clients.User(receiverId)
         .SendAsync("DeleteMessage", message,ct);
   }
}