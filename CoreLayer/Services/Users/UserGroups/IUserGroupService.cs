using CoreLayer.ViewModels.Chats;

namespace CoreLayer.Services.Users.UserGroups;

public interface IUserGroupService
{
    Task<List<UserGroupViewModel>> GetUserGroups(long userId);
    Task JoinGroup(long userId, long groupId);
    Task JoinGroup(List<long> userIds, long groupId);
    Task<bool> IsUserInGroup(long userId, long groupId);
    Task<bool> IsUserInGroup(long userId, string groupToken);
    Task<List<string>> GetUserIdsByGroupId(long groupId);
}
