namespace CoreLayer.ViewModels.Chats
{
    public class ChatViewModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string GroupName { get; set; }
        public long GroupId { get; set; }
        public string ChatBody { get; set; }
        public string CreationDate { get; set; }
    }
}
