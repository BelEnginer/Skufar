using Application.DTOs.GetDtos;

namespace Application.Abstractions.IHubs;

public interface IChatHub
{
    
    Task SendMessage(string receiverId,MessageDto message,CancellationToken ct);
    Task DeleteMessage(string receiverId,MessageDto message,CancellationToken ct);
}