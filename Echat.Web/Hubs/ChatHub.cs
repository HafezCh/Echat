using CoreLayer.Services.Chats;
using CoreLayer.Services.Chats.ChatGroups;
using CoreLayer.Services.Users.UserGroups;
using CoreLayer.Utilities;
using CoreLayer.ViewModels.Chats;
using DataLayer.Entities.Chats;
using Microsoft.AspNetCore.SignalR;

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
            Clients.Caller.SendAsync("Welcome", Context.User.GetUserId());
            return base.OnConnectedAsync();
        }

        public async Task JoinGroup(string token, long currentGroupId)
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

            if (currentGroupId > 0)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentGroupId.ToString());

            await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());

            var chats = await _chatService.GetGroupChats(group.Id);

            await Clients.Group(group.Id.ToString()).SendAsync("JoinGroup", group, chats);
        }

        public async Task SendMessage(string text, long groupId)
        {
            var group = await _chatGroupService.GetGroupById(groupId);

            if (group == null) return;

            var chat = new Chat
            {
                ChatBody = text,
                GroupId = groupId,
                UserId = Context.User.GetUserId()
            };

            await _chatService.SendMessage(chat);

            var chatModel = new ChatViewModel
            {
                GroupId = groupId,
                ChatBody = chat.ChatBody,
                UserId = chat.UserId,
                GroupName = group.GroupTitle,
                UserName = Context.User!.Identity!.Name!,
                CreationDate = $"{chat.CreationDate.Hour}:{chat.CreationDate.Minute} | {chat.CreationDate.ToShortDateString()}"
            };

            var userIds = await _userGroupService.GetUserIdsByGroupId(groupId);

            await Clients.Users(userIds).SendAsync("ReceiveNotification", chatModel);

            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", chatModel);
        }
    }
}
