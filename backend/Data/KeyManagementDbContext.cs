using System.Security.Claims;
using System.Text.Json;
using KeyManagement.Api.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Data
{
    public class KeyManagementDbContext : DbContext
    {
        private static readonly string[] SensitivePropertyMarkers =
            ["password", "hash", "pin", "token", "secret", "credential", "securitystamp"];
        private static readonly string[] FreeTextPropertyMarkers =
            ["description", "comment", "notes", "content", "message", "body"];

        private readonly IHttpContextAccessor? _httpContextAccessor;

        public KeyManagementDbContext(
            DbContextOptions<KeyManagementDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
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
        public DbSet<AuditLog> AuditLogs { get; set; }

        public override int SaveChanges()
        {
            return SaveChanges(acceptAllChangesOnSuccess: true);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess).GetAwaiter().GetResult();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            var changes = CaptureChanges();
            if (changes.Count == 0)
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }

            var transaction = Database.CurrentTransaction;
            var ownsTransaction = transaction is null && Database.IsRelational();
            if (ownsTransaction)
            {
                transaction = await Database.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                var savedChanges = await base.SaveChangesAsync(
                    acceptAllChangesOnSuccess,
                    cancellationToken);

                var occurredAt = DateTime.UtcNow;
                var auditEntries = changes
                    .Select(change => CreateAuditEntry(change, occurredAt))
                    .ToList();

                if (!acceptAllChangesOnSuccess)
                {
                    foreach (var change in changes)
                    {
                        change.Entry.State = EntityState.Unchanged;
                    }
                }

                AuditLogs.AddRange(auditEntries);
                var auditChanges = await base.SaveChangesAsync(
                    acceptAllChangesOnSuccess: true,
                    cancellationToken);

                if (!acceptAllChangesOnSuccess)
                {
                    RestoreUnacceptedChanges(changes);
                }

                if (ownsTransaction)
                {
                    await transaction!.CommitAsync(cancellationToken);
                }

                return savedChanges + auditChanges;
            }
            catch
            {
                if (ownsTransaction)
                {
                    await transaction!.RollbackAsync(CancellationToken.None);
                }

                throw;
            }
            finally
            {
                if (ownsTransaction)
                {
                    await transaction!.DisposeAsync();
                }
            }
        }

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

            modelBuilder.Entity<Key>()
                .HasIndex(k => k.Code)
                .IsUnique();

            modelBuilder.Entity<KeyTransaction>()
                .HasIndex(transaction => new { transaction.EndedAt, transaction.DueAt });

            modelBuilder.Entity<AuditLog>()
                .HasIndex(entry => entry.OccurredAt);
            modelBuilder.Entity<AuditLog>()
                .HasIndex(entry => new { entry.ActorUserId, entry.OccurredAt });
            modelBuilder.Entity<AuditLog>()
                .HasIndex(entry => new { entry.EventType, entry.OccurredAt });

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

        private List<PendingAuditChange> CaptureChanges()
        {
            return ChangeTracker.Entries()
                .Where(entry =>
                    entry.Entity is not AuditLog &&
                    entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .Select(entry =>
                {
                    var state = entry.State;
                    var keyPropertyNames = entry.Metadata.FindPrimaryKey()?.Properties
                        .Select(property => property.Name)
                        .ToHashSet(StringComparer.Ordinal) ?? new HashSet<string>(StringComparer.Ordinal);
                    var properties = entry.Properties
                        .Where(property =>
                            !keyPropertyNames.Contains(property.Metadata.Name) &&
                            (state is EntityState.Added or EntityState.Deleted || property.IsModified))
                        .Select(property => new AuditPropertyChange(
                            property.Metadata.Name,
                            property.OriginalValue,
                            property.CurrentValue))
                        .ToList();
                    var originalValues = entry.Properties.ToDictionary(
                        property => property.Metadata.Name,
                        property => property.OriginalValue,
                        StringComparer.Ordinal);
                    var modifiedProperties = entry.Properties
                        .Where(property => property.IsModified)
                        .Select(property => property.Metadata.Name)
                        .ToHashSet(StringComparer.Ordinal);

                    return new PendingAuditChange(
                        entry,
                        state,
                        properties,
                        originalValues,
                        modifiedProperties);
                })
                .ToList();
        }

        private AuditLog CreateAuditEntry(PendingAuditChange change, DateTime occurredAt)
        {
            var context = _httpContextAccessor?.HttpContext;
            var principal = context?.User;
            var userIdClaim = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            int? actorUserId = int.TryParse(userIdClaim, out var parsedUserId)
                ? parsedUserId
                : null;
            var entityName = change.Entry.Metadata.ClrType.Name;
            var entityId = GetEntityId(change.Entry);
            var actorRoles = principal is null
                ? string.Empty
                : string.Join(
                    ",",
                    principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).Distinct());
            var action = change.State switch
            {
                EntityState.Added => "CREATE",
                EntityState.Modified => "UPDATE",
                EntityState.Deleted => "DELETE",
                _ => throw new InvalidOperationException("Unsupported audited entity state.")
            };

            return new AuditLog
            {
                EventType = AuditLogEventTypes.DataChange,
                ActorUserId = actorUserId,
                ActorEmail = Truncate(principal?.FindFirstValue(ClaimTypes.Email), 256),
                ActorRoles = Truncate(actorRoles, 256),
                Method = action,
                Resource = Truncate($"{entityName}/{entityId ?? "unknown"}", 512) ?? "unknown",
                EntityName = Truncate(entityName, 128),
                EntityId = Truncate(entityId, 256),
                Details = JsonSerializer.Serialize(change.Properties.Select(property => new
                {
                    field = property.Name,
                    oldValue = change.State == EntityState.Added
                        ? null
                        : GetSafeAuditValue(property.Name, property.OriginalValue),
                    newValue = change.State == EntityState.Deleted
                        ? null
                        : GetSafeAuditValue(property.Name, property.CurrentValue)
                })),
                CorrelationId = Truncate(context?.TraceIdentifier, 64),
                StatusCode = StatusCodes.Status200OK,
                OccurredAt = occurredAt,
                IpAddress = Truncate(context?.Connection.RemoteIpAddress?.ToString(), 45),
                UserAgent = Truncate(context?.Request.Headers.UserAgent.ToString(), 512),
                DurationMilliseconds = 0
            };
        }

        private static string? GetEntityId(EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();
            if (key is null)
            {
                return null;
            }

            var values = key.Properties.ToDictionary(
                property => property.Name,
                property => entry.Property(property.Name).CurrentValue);
            return JsonSerializer.Serialize(values);
        }

        private static object? GetSafeAuditValue(string propertyName, object? value)
        {
            if (value is null)
            {
                return null;
            }

            if (IsSensitiveProperty(propertyName))
            {
                return "[REDACTED]";
            }

            if (value is string text)
            {
                if (IsFreeTextProperty(propertyName))
                {
                    return $"[OMITTED; {text.Length} characters]";
                }

                return text.Length <= 256 ? text : $"{text[..256]} [TRUNCATED]";
            }

            if (value is byte[] bytes)
            {
                return $"[OMITTED; {bytes.Length} bytes]";
            }

            return value;
        }

        private static bool IsSensitiveProperty(string propertyName)
        {
            return SensitivePropertyMarkers.Any(marker =>
                propertyName.Contains(marker, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsFreeTextProperty(string propertyName)
        {
            return FreeTextPropertyMarkers.Any(marker =>
                propertyName.Contains(marker, StringComparison.OrdinalIgnoreCase));
        }

        private static string? Truncate(string? value, int maximumLength)
        {
            return value is { Length: > 0 }
                ? value.Length <= maximumLength ? value : value[..maximumLength]
                : null;
        }

        private static void RestoreUnacceptedChanges(IEnumerable<PendingAuditChange> changes)
        {
            foreach (var change in changes)
            {
                var entry = change.Entry;
                entry.State = EntityState.Unchanged;

                if (change.State == EntityState.Modified)
                {
                    foreach (var originalValue in change.OriginalValues)
                    {
                        entry.Property(originalValue.Key).OriginalValue = originalValue.Value;
                    }

                    foreach (var propertyName in change.ModifiedProperties)
                    {
                        entry.Property(propertyName).IsModified = true;
                    }
                }
                else
                {
                    entry.State = change.State;
                }
            }
        }

        private sealed record AuditPropertyChange(string Name, object? OriginalValue, object? CurrentValue);

        private sealed record PendingAuditChange(
            EntityEntry Entry,
            EntityState State,
            IReadOnlyList<AuditPropertyChange> Properties,
            IReadOnlyDictionary<string, object?> OriginalValues,
            IReadOnlySet<string> ModifiedProperties);
    }
}
