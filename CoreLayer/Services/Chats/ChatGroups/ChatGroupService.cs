using CoreLayer.Services.Users.UserGroups;
using CoreLayer.Utilities;
using CoreLayer.ViewModels.Chats;
using DataLayer.Context;
using DataLayer.Entities.Chats;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CoreLayer.Services.Chats.ChatGroups;

public class ChatGroupService : BaseService, IChatGroupService
{
    private readonly IUserGroupService _userGroupService;

    public ChatGroupService(EChatApplicationContext context, IUserGroupService userGroupService) : base(context)
    {
        _userGroupService = userGroupService;
    }

    public async Task<List<ChatGroup>> GetUsersGroups(long userId)
    {
        return await Table<ChatGroup>()
            .Include(x => x.Chats)
            .Where(x => x.OwnerId == userId)
            .OrderByDescending(x => x.CreationDate)
            .ToListAsync();
    }

    public async Task<ChatGroup> InsertGroup(CreateGroupViewModel model)
    {
        if (!FileValidation.IsValidImageFile(model.ImageFile.FileName))
            throw new ValidationException();

        var imageName = await model.ImageFile.SaveFile("wwwroot/images/groups");

        var chatGroup = new ChatGroup
        {
            GroupTitle = model.GroupName,
            GroupToken = Guid.NewGuid().ToString(),
            OwnerId = model.UserId,
            ImageName = imageName
        };

        Insert(chatGroup);
        await Save();

        await _userGroupService.JoinGroup(model.UserId, chatGroup.Id);

        return chatGroup;
    }
}