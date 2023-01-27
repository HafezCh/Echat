using CoreLayer.ViewModels.Chats;
using DataLayer.Context;
using DataLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace CoreLayer.Services.Users.UserGroups;

public class UserGroupService : BaseService, IUserGroupService
{
    public UserGroupService(EChatApplicationContext context) : base(context)
    {
    }

    public async Task<List<UserGroupViewModel>> GetUserGroups(long userId)
    {
        var result = await Table<UserGroup>()
            .Include(x => x.ChatGroup)
            .ThenInclude(x => x.Chats)
            .Where(x => x.UserId == userId)
            .Select(x => new UserGroupViewModel
            {
                Token = x.ChatGroup.GroupToken,
                ImageName = x.ChatGroup.ImageName,
                GroupName = x.ChatGroup.GroupTitle,
                LastChat = x.ChatGroup.Chats.OrderBy(c => c.CreationDate).LastOrDefault()
            }).AsNoTracking().ToListAsync();

        return result;
    }

    public async Task JoinGroup(long userId, long groupId)
    {
        Insert(new UserGroup
        {
            UserId = userId,
            GroupId = groupId
        });
        await Save();
    }

    public async Task<bool> IsUserInGroup(long userId, long groupId)
    {
        return await Table<UserGroup>().AnyAsync(x => x.UserId == userId && x.GroupId == groupId);
    }

    public async Task<bool> IsUserInGroup(long userId, string groupToken)
    {
        return await Table<UserGroup>()
            .Include(x => x.ChatGroup)
            .AnyAsync(x => x.UserId == userId && x.ChatGroup.GroupToken == groupToken);
    }

    public async Task<List<string>> GetUserIdsByGroupId(long groupId)
    {
        return await Table<UserGroup>()
            .Where(x => x.GroupId == groupId)
            .Select(x => x.UserId.ToString())
            .ToListAsync();
    }
}