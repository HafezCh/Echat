namespace Echat.Web.Hubs;

public interface IChatHub
{
    Task JoinGroup(string token, long currentGroupId);
    Task SendMessage(string text, long groupId);
    Task JoinPrivateGroup(long receiverId, long currentGroupId);
}