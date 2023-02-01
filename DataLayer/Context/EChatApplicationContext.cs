using DataLayer.Entities.Chats;
using DataLayer.Entities.Roles;
using DataLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context;

public class EChatApplicationContext : DbContext
{
    public EChatApplicationContext(DbContextOptions<EChatApplicationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //var cascades = modelBuilder.Model.GetEntityTypes()
        //    .SelectMany(t => t.GetForeignKeys())
        //    .Where(fk => fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        //foreach (var fk in cascades)
        //{
        //    fk.DeleteBehavior = DeleteBehavior.Restrict;
        //}

        modelBuilder.Entity<Chat>()
            .HasOne(b => b.User)
            .WithMany(b => b.Chats)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserGroup>()
            .HasOne(b => b.User)
            .WithMany(b => b.UserGroups)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChatGroup>()
            .HasOne(b => b.Receiver)
            .WithMany(b => b.PrivateGroups)
            .HasForeignKey(b => b.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatGroup> ChatGroups { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
}