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
}