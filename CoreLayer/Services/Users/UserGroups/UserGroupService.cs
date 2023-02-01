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
            .Include(x => x.ChatGroup.Chats)
            .Include(x => x.ChatGroup.Receiver)
            .Include(x => x.ChatGroup.User)
            .Where(x => x.UserId == userId)
            .AsNoTracking().ToListAsync();

        var model = new List<UserGroupViewModel>();

        result.ForEach(userGroup =>
        {
            var chatGroup = userGroup.ChatGroup;
            if (chatGroup.ReceiverId != null)
            {
                if (chatGroup.ReceiverId == userId)
                    model.Add(new UserGroupViewModel
                    {
                        ImageName = chatGroup.User.AvatarName,
                        GroupName = chatGroup.User.UserName,
                        Token = chatGroup.GroupToken,
                        LastChat = chatGroup.Chats.OrderByDescending(x => x.CreationDate).FirstOrDefault()
                    });
                else
                    model.Add(new UserGroupViewModel
                    {
                        ImageName = chatGroup.Receiver.AvatarName,
                        GroupName = chatGroup.Receiver.UserName,
                        Token = chatGroup.GroupToken,
                        LastChat = chatGroup.Chats.OrderByDescending(x => x.CreationDate).FirstOrDefault()
                    });
            }
            else
            {
                model.Add(new UserGroupViewModel
                {
                    ImageName = chatGroup.ImageName,
                    GroupName = chatGroup.GroupTitle,
                    Token = chatGroup.GroupToken,
                    LastChat = chatGroup.Chats.OrderByDescending(x => x.CreationDate).FirstOrDefault()
                });
            }
        });

        return model;
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

    public async Task JoinGroup(List<long> userIds, long groupId)
    {
        userIds.ForEach(userId =>
        {
            Insert(new UserGroup
            {
                UserId = userId,
                GroupId = groupId
            });
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