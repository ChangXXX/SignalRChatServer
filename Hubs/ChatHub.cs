using SignalRChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace SignalRChat.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public static string path = "/chatHub";
    private List<string> _users = new List<string>();

    public override async Task OnConnectedAsync()
    {
        string name = Context.User.Claims.FirstOrDefault().Value;
        _users.Add(name);
        await Clients.All.SendAsync("ReceiveSystemMessage", $"{name} joined.");
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        string name = Context.User.Claims.FirstOrDefault().Value;
        if (_users.Contains(name))
        {
            _users.Remove(name);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToAll(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveAllMessage", user, message);
    }
}
