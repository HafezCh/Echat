using Microsoft.AspNetCore.Http;

namespace CoreLayer.ViewModels.Chats;

public class CreateGroupViewModel
{
    public long UserId { get; set; }
    public string GroupName { get; set; }
    public IFormFile ImageFile { get; set; }
}