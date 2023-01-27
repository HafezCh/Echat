using CoreLayer.ViewModels.Chats;
using DataLayer.Entities.Chats;

namespace CoreLayer.Services.Chats
{
    public interface IChatService
    {
        Task SendMessage(Chat chat);
        Task<List<ChatViewModel>> GetGroupChats(long groupId);
    }
}