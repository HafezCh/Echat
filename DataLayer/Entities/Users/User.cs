using DataLayer.Entities.Chats;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities.Users;

public class User : BaseEntity
{
    [MaxLength(50)]
    public string UserName { get; set; }

    [MinLength(6)]
    [MaxLength(50)]
    public string Password { get; set; }

    [MaxLength(110)]
    public string AvatarName { get; set; }

    #region Relations

    public ICollection<ChatGroup> ChatGroups { get; set; }
    public ICollection<Chat> Chats { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }

    #endregion
}