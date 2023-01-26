using CoreLayer.Services.Chats.ChatGroups;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Echat.Web.Hubs
{
    public class ChatHub : Hub, IChatHub
    {
        private readonly IChatGroupService _chatGroupService;

        public ChatHub(IChatGroupService chatGroupService)
        {
            _chatGroupService = chatGroupService;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string text)
        {
            var userName = Context.User.FindFirstValue(ClaimTypes.Name);
            await Clients.Others.SendAsync("ReceiveMessage", $"{userName} : {text}");
        }
    }
}
