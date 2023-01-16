using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities;

public class BaseEntity
{
    [Key]
    public long Id { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
}