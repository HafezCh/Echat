using CoreLayer.Services.Chats.ChatGroups;
using CoreLayer.Services.Users.UserGroups;
using CoreLayer.Utilities;
using CoreLayer.ViewModels.Chats;
using Echat.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Echat.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatGroupService _chatGroupService;
        private readonly IUserGroupService _userGroupService;

        public HomeController(IChatGroupService chatGroupService, IHubContext<ChatHub> hubContext, IUserGroupService userGroupService)
        {
            _chatGroupService = chatGroupService;
            _hubContext = hubContext;
            _userGroupService = userGroupService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _userGroupService.GetUserGroups(User.GetUserId());

            return View(model);
        }

        [HttpPost]
        public async Task CreateGroup([FromForm] CreateGroupViewModel? model)
        {
            if (model == null)
            {
                await _hubContext.Clients.User(User.GetUserId().ToString()).SendAsync("NewGroup", "Error");
            }

            model.UserId = User.GetUserId();

            try
            {
                var result = await _chatGroupService.InsertGroup(model);
                await _hubContext.Clients.User(model.UserId.ToString()).SendAsync("NewGroup", result.GroupTitle, result.GroupToken, result.ImageName);
            }
            catch
            {
                await _hubContext.Clients.User(model.UserId.ToString()).SendAsync("NewGroup", "Error");
            }
        }

        public async Task<IActionResult> Search(string title)
        {
            return new ObjectResult(await _chatGroupService.Search(title, User.GetUserId()));
        }
    }
}