using CoreLayer.ViewModels.Chats;
using DataLayer.Context;
using DataLayer.Entities.Chats;
using Microsoft.EntityFrameworkCore;

namespace CoreLayer.Services.Chats
{
    public class ChatService : BaseService, IChatService
    {
        public ChatService(EChatApplicationContext context) : base(context)
        {
        }

        public async Task SendMessage(Chat chat)
        {
            Insert(chat);
            await Save();
        }

        public async Task<List<ChatViewModel>> GetGroupChats(long groupId)
        {
            return await Table<Chat>()
                .Include(x => x.ChatGroup)
                .Include(x => x.User)
                .Where(x => x.GroupId == groupId)
                .Select(x => new ChatViewModel
                {
                    ChatBody = x.ChatBody,
                    UserId = x.UserId,
                    GroupId = x.GroupId,
                    GroupName = x.ChatGroup.GroupTitle,
                    UserName = x.User.UserName,
                    CreationDate = $"{x.CreationDate.Hour}:{x.CreationDate.Minute} | {x.CreationDate.ToShortDateString()}",
                }).AsNoTracking().ToListAsync();
        }
    }
}