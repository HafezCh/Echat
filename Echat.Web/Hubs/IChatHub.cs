namespace Echat.Web.Hubs;

public interface IChatHub
{
    Task JoinGroup(string token);
}