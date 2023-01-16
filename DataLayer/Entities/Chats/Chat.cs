using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.Users;

namespace DataLayer.Entities.Chats;

public class Chat : BaseEntity
{
    public string ChatBody { get; set; }
    public long UserId { get; set; }
    public long GroupId { get; set; }

    #region Relations

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    [ForeignKey(nameof(GroupId))]
    public ChatGroup ChatGroup { get; set; }

    #endregion
}