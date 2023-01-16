using DataLayer.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities.Roles;

public class RolePermission : BaseEntity
{
    public long RoleId { get; set; }
    public Permission Permission { get; set; }

    #region Relations

    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; }

    #endregion
}