using Microsoft.EntityFrameworkCore;
using KeyManagement.Api.Models.Entities;

namespace KeyManagement.Api.Data
{
public class KeyManagementDbContext : DbContext
    {
    public KeyManagementDbContext(DbContextOptions<KeyManagementDbContext> options) : base(options)
    {
    }

    public DbSet<Building> Buildings { get; set; }
    public DbSet<Key> Keys { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<KeyTransaction> KeyTransactions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Equipment> Equipments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Building>()
            .HasMany(r => r.Rooms)
            .WithOne(b => b.Building)
            .HasForeignKey(r => r.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasMany(r => r.Equipments)
            .WithOne(e => e.Room)
            .HasForeignKey(e => e.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasMany(r => r.Reservations)
            .WithOne(r => r.Room)
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasMany(k => k.Keys)
            .WithOne(r => r.Room)
            .HasForeignKey(k => k.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.UserRoles)
            .WithOne(r => r.User)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Requester)
            .WithMany()
            .HasForeignKey(r => r.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Approver)
            .WithMany()
            .HasForeignKey(r => r.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

       modelBuilder.Entity<KeyTransaction>()
            .HasOne(kt => kt.HolderUser)
            .WithMany()
            .HasForeignKey(kt => kt.HolderUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<KeyTransaction>()
            .HasOne(kt => kt.IssuedByUser)
            .WithMany()
            .HasForeignKey(kt => kt.IssuedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<KeyTransaction>()
            .HasOne(kt => kt.ReturnedToUser)
            .WithMany()
            .HasForeignKey(kt => kt.ReturnedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>()
            .HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(r => r.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(al => al.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

    }
    }
}