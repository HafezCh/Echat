using System.Threading.Tasks;
using DataLayer.Entities.Chats;

namespace CoreLayer.Services.Chats
{
    public interface IChatService
    {
        Task SendMessage(Chat chat);
    }
}