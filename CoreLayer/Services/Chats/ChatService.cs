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
                .Where(x => x.GroupId == groupId)
                .Select(x => new ChatViewModel
                {
                    Text = x.ChatBody,
                    UserId = x.UserId,
                    GroupId = x.GroupId,
                    CreationDate = x.CreationDate.ToShortDateString(),
                }).AsNoTracking().ToListAsync();
        }
    }
}