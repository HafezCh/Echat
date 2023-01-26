using CoreLayer.ViewModels.Chats;

namespace CoreLayer.Services.Users.UserGroups;

public interface IUserGroupService
{
    Task<List<UserGroupViewModel>> GetUserGroups(long userId);
    Task JoinGroup(long userId, long groupId);
}
