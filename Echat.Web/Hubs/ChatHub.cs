using CoreLayer.Services.Chats;
using CoreLayer.Services.Chats.ChatGroups;
using CoreLayer.Services.Users.UserGroups;
using CoreLayer.Utilities;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Echat.Web.Hubs
{
    public class ChatHub : Hub, IChatHub
    {
        private readonly IChatService _chatService;
        private readonly IChatGroupService _chatGroupService;
        private readonly IUserGroupService _userGroupService;

        public ChatHub(IChatGroupService chatGroupService, IUserGroupService userGroupService, IChatService chatService)
        {
            _chatGroupService = chatGroupService;
            _userGroupService = userGroupService;
            _chatService = chatService;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string text)
        {
            var userName = Context.User.FindFirstValue(ClaimTypes.Name);
            await Clients.Others.SendAsync("ReceiveMessage", $"{userName} : {text}");
        }

        public async Task JoinGroup(string token)
        {
            var group = await _chatGroupService.GetGroupByToken(token);

            if (group == null)
            {
                await Clients.Caller.SendAsync("Error", "GroupNotFound");
                return;
            }

            if (!await _userGroupService.IsUserInGroup(Context.User.GetUserId(), token))
            {
                await _userGroupService.JoinGroup(Context.User.GetUserId(), group.Id);
                await Clients.Caller.SendAsync("NewGroup", group.GroupTitle, group.GroupToken, group.ImageName);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());

            await Clients.Group(group.Id.ToString()).SendAsync("JoinGroup", group, group.Chats);
        }
    }
}
