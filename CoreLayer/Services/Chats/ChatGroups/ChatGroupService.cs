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

    public async Task<List<SearchResultViewModel>> Search(string title, long userId)
    {
        var result = new List<SearchResultViewModel>();

        if (string.IsNullOrWhiteSpace(title))
            return result;

        var groups = await Table<ChatGroup>()
            .Where(x => x.GroupTitle.Contains(title) && !x.IsPrivate)
            .Select(x => new SearchResultViewModel
            {
                ImageName = x.ImageName,
                Token = x.GroupToken,
                Title = x.GroupTitle,
                IsUser = false
            }).ToListAsync();

        var users = await Table<User>()
            .Where(x => x.UserName.Contains(title) && x.Id != userId)
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
        return await Table<ChatGroup>()
            .Include(x => x.User)
            .Include(x => x.Receiver)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ChatGroup?> GetGroupByToken(string token)
    {
        return await Table<ChatGroup>()
            .Include(x => x.User)
            .Include(x => x.Receiver)
            .FirstOrDefaultAsync(x => x.GroupToken == token);
    }

    public async Task<ChatGroup> InsertPrivateGroup(long userId, long receiverId)
    {
        var group = await Table<ChatGroup>()
            .Include(x => x.User)
            .Include(x => x.Receiver)
            .SingleOrDefaultAsync(x => (x.OwnerId == userId && x.ReceiverId == receiverId)
                                       || (x.OwnerId == receiverId && x.ReceiverId == userId));

        if (group != null) return group;

        var groupCreated = new ChatGroup
        {
            GroupTitle = $"Chat With {receiverId}",
            GroupToken = Guid.NewGuid().ToString(),
            ImageName = "Default.jpg",
            IsPrivate = true,
            OwnerId = userId,
            ReceiverId = receiverId
        };

        Insert(groupCreated);
        await Save();
        return (await GetGroupById(groupCreated.Id))!;
    }
}