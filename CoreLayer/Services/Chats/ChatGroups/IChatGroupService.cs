﻿using CoreLayer.ViewModels.Chats;
using DataLayer.Entities.Chats;

namespace CoreLayer.Services.Chats.ChatGroups;

public interface IChatGroupService
{
    Task<List<ChatGroup>> GetUsersGroups(long userId);
    Task<ChatGroup> InsertGroup(CreateGroupViewModel model);
    Task<List<SearchResultViewModel>> Search(string title, long userId);
    Task<ChatGroup?> GetGroupById(long id);
    Task<ChatGroup?> GetGroupByToken(string token);
    Task<ChatGroup> InsertPrivateGroup(long userId, long receiverId);
}