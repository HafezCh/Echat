using DataLayer.Entities.Chats;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities.Users;

public class UserGroup : BaseEntity
{
    public long UserId { get; set; }
    public long GroupId { get; set; }

    #region Relations

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    [ForeignKey(nameof(GroupId))]
    public ChatGroup ChatGroup { get; set; }

    #endregion
}