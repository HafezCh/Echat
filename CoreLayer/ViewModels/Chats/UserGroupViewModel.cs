using DataLayer.Entities.Chats;

namespace CoreLayer.ViewModels.Chats
{
    public class UserGroupViewModel
    {
        public string GroupName { get; set; }
        public string ImageName { get; set; }
        public string Token { get; set; }
        public Chat? LastChat { get; set; }
    }
}
