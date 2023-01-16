using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.Users;

namespace DataLayer.Entities.Chats;

public class ChatGroup : BaseEntity
{
    [MaxLength(100)]
    public string GroupTitle { get; set; }
    [MaxLength(110)]
    public string GroupToken { get; set; }

    public long OwnerId { get; set; }

    #region Relations

    [ForeignKey(nameof(OwnerId))]
    public User User { get; set; }
    public ICollection<Chat> Chats { get; set; }

    #endregion
}