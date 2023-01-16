using DataLayer.Context;
using DataLayer.Entities.Chats;

namespace CoreLayer.Services.Chats;

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
}