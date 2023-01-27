using CoreLayer.Services.Users.UserGroups;
using CoreLayer.Utilities;
using CoreLayer.ViewModels.Chats;
using DataLayer.Context;
using DataLayer.Entities.Chats;
using DataLayer.Entities.Users;
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

    public async Task<List<SearchResultViewModel>> Search(string title)
    {
        var result = new List<SearchResultViewModel>();

        if (string.IsNullOrWhiteSpace(title))
            return result;

        var groups = await Table<ChatGroup>()
            .Where(x => x.GroupTitle.Contains(title))
            .Select(x => new SearchResultViewModel
            {
                ImageName = x.ImageName,
                Token = x.GroupToken,
                Title = x.GroupTitle,
                IsUser = false
            }).ToListAsync();

        var users = await Table<User>()
            .Where(x => x.UserName.Contains(title))
            .Select(x => new SearchResultViewModel
            {
                ImageName = x.AvatarName,
                Token = x.Id.ToString(),
                Title = x.UserName,
                IsUser = true
            }).ToListAsync();

        result.AddRange(groups);
        result.AddRange(users);

        return result;
    }

    public async Task<ChatGroup?> GetGroupById(long id)
    {
        return await GetById<ChatGroup>(id, "x=>x.Chats");
    }

    public async Task<ChatGroup?> GetGroupByToken(string token)
    {
        return await Table<ChatGroup>()
            .Include(x => x.Chats)
            .FirstOrDefaultAsync(x => x.GroupToken == token);
    }
}