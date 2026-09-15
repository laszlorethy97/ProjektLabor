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
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<KeyTransaction> KeyTransactions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<MasterKey> MasterKeys { get; set; }
        public DbSet<MasterKeyTransaction> MasterKeyTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Building)
                .WithMany(b => b.Rooms)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Room)
                .WithMany(r => r.Equipments)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.CreatedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Key>()
                .HasOne(k => k.Room)
                .WithMany(r => r.Keys)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Key>()
                .HasOne(k => k.CreatedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.CreatedBy)
                .WithMany()
                .HasForeignKey(u => u.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.CreatedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Permission>()
                .HasOne(p => p.User)
                .WithMany(u => u.Permissions)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Permission>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Permission>()
                .HasOne(p => p.MasterKey)
                .WithMany(mk => mk.Permissions)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MasterKey>()
                .HasOne(mk => mk.CreatedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KeyTransaction>()
                .HasOne(kt => kt.Key)
                .WithMany(k => k.KeyTransactions)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KeyTransaction>()
                .HasOne(kt => kt.User)
                .WithMany(u => u.KeyTransactions)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KeyTransaction>()
                .HasOne(kt => kt.StartedByUser)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KeyTransaction>()
                .HasOne(kt => kt.EndedByUser)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MasterKeyTransaction>()
                .HasOne(mkt => mkt.MasterKey)
                .WithMany(mk => mk.MasterKeyTransactions)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MasterKeyTransaction>()
                .HasOne(mkt => mkt.User)
                .WithMany(u => u.MasterKeyTransactions)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MasterKeyTransaction>()
                .HasOne(mkt => mkt.StartedByUser)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MasterKeyTransaction>()
                .HasOne(mkt => mkt.EndedByUser)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MasterKeyTransaction>()
                .HasOne(mkt => mkt.ApprovedByUser)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Room)
                .WithMany(rm => rm.Reservations)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Room)
                .WithMany(r => r.Maintenances)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.User)
                .WithMany(u => u.Maintenances)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.Tickets)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
