using SignalRChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;
using MongoDB.Bson;
using SignalRChat.Contexts;
using Microsoft.EntityFrameworkCore;

namespace SignalRChat.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public static string path = "/chatHub";
    private readonly RoomContext _roomContext;
    private long _roomId = -1;
    private static int _roomLimit = 4;

    public ChatHub(RoomContext roomContext)
    {
        _roomContext = roomContext;
    }


    public override async Task OnConnectedAsync()
    {
        string name = Context.User.Claims.FirstOrDefault().Value;
        // ������ ã�Ƽ� �ش� �濡 �����Ŵ.
        var _rooms = _roomContext.Rooms
            .Select(x => new Room(x.Id, x.Users))
            .ToList();
        for (int i = 0; i < _rooms.Count; i++)
        {
            var room = _rooms[i];
            if (room.Users.Contains(name))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
                await Clients.Group(room.Id.ToString()).SendAsync("SystemMessage", $"{name} �����ϼ̽��ϴ�.");
                break;
            }
        }

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        string name = Context.User.Claims.FirstOrDefault().Value;
        // ������ ã�Ƽ� �ش� �濡�� ������ ����.
        var _rooms = _roomContext.Rooms
            .Select(x => new Room(x.Id, x.Users))
            .ToList();

        for (int i = 0; i < _rooms.Count; i++)
        {
            var room = _rooms[i];
            if (room.Users.Contains(name))
            {
                Groups.RemoveFromGroupAsync(Context.ConnectionId, name);
                Clients.Group(room.Id.ToString()).SendAsync("SystemMessage", $"{name} �����ϼ̽��ϴ�.").GetAwaiter().GetResult();
                break;
            }
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToAll(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveAllMessage", user, message);
    }

    public async Task CreateRoom(List<string> users)
    {
        var cnt = _roomContext.Rooms
            .Select(x => new Room(x.Id, x.Users))
            .ToList()
            .Count;
        long id = -1;

        if (cnt < _roomLimit)
        {
            string name = Context.User.Claims.FirstOrDefault().Value;
            id = Interlocked.Increment(ref _roomId);
            var newRoom = new Room(id, users);
            _roomContext.Rooms.Add(newRoom);
            await _roomContext.SaveChangesAsync();
            await Clients.All.SendAsync("SystemMessage", $"{name}���� ���� ����̽��ϴ�");
        }
        else
        {
            await Clients.Client(Context.ConnectionId).SendAsync("SystemMessage", "�� ������ �����Ͽ����ϴ�");
        }
    }

    public async Task SendMessage(Room room, string message)
    {
        string name = Context.User.Claims.FirstOrDefault().Value;
        await Clients.Group(room.Id.ToString()).SendAsync($"{name} : {message}");
    }

    public async Task EnterManyUserRoom()
    {
        string name = Context.User.Claims.FirstOrDefault().Value;
        var _rooms = _roomContext.Rooms
            .Select(x => new Room(x.Id, x.Users))
            .ToList();

        if (_rooms.Count > 0)
        {
            Room room = _rooms[0];
            for (int i = 1; i < _rooms.Count; i++)
            {
                if (room.Users.Count < _rooms[i].Users.Count)
                {
                    room = _rooms[i];
                }
            }
            _roomContext.Rooms.Update(room);
            await _roomContext.SaveChangesAsync();
            var userNames = String.Join(", ", room.Users);

            await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
            await Clients.Group(room.Id.ToString()).SendAsync("SystemMessage", $"{name}���� �����ϼ̽��ϴ�.");
            await Clients.Group(room.Id.ToString()).SendAsync("SystemMessage", $"���� ������ ��� ::: {userNames}");
        }
        else
        {
            await Clients.User(Context.User.Identity.Name).SendAsync("SystemMessage", "ä�ù��� �������� �ʽ��ϴ�");
        }
    }
}
