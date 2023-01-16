using DataLayer.Entities.Roles;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities.Users;

public class UserRole : BaseEntity
{
    public long RoleId { get; set; }
    public long UserId { get; set; }

    #region Relations

    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    #endregion
}