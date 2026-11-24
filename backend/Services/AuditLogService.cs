using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services;

public sealed class AuditLogService : IAuditLogService
{
    private readonly KeyManagementDbContext _context;

    public AuditLogService(KeyManagementDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLogPageResponse> GetEventsAsync(
        AuditLogQuery query,
        CancellationToken cancellationToken)
    {
        var events = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.EventType))
        {
            events = events.Where(entry => entry.EventType == query.EventType);
        }

        if (query.ActorUserId is int actorUserId)
        {
            events = events.Where(entry => entry.ActorUserId == actorUserId);
        }

        if (query.FromDate is DateOnly fromDate)
        {
            var start = DateTime.SpecifyKind(fromDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            events = events.Where(entry => entry.OccurredAt >= start);
        }

        if (query.ToDate is DateOnly toDate)
        {
            var endExclusive = DateTime.SpecifyKind(
                toDate.AddDays(1).ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc);
            events = events.Where(entry => entry.OccurredAt < endExclusive);
        }

        var search = query.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
        {
            events = events.Where(entry =>
                entry.EventType.Contains(search) ||
                entry.Method.Contains(search) ||
                entry.Resource.Contains(search) ||
                (entry.ActorEmail != null && entry.ActorEmail.Contains(search)) ||
                (entry.EntityName != null && entry.EntityName.Contains(search)) ||
                (entry.EntityId != null && entry.EntityId.Contains(search)));
        }

        var totalCount = await events.CountAsync(cancellationToken);
        var entries = await events
            .OrderByDescending(entry => entry.OccurredAt)
            .ThenByDescending(entry => entry.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(entry => new
            {
                entry.Id,
                entry.EventType,
                entry.ActorUserId,
                entry.ActorEmail,
                entry.ActorRoles,
                entry.Method,
                entry.Resource,
                entry.StatusCode,
                entry.OccurredAt,
                entry.IpAddress,
                entry.UserAgent,
                entry.DurationMilliseconds,
                entry.EntityName,
                entry.EntityId,
                entry.Details,
                entry.CorrelationId
            })
            .ToListAsync(cancellationToken);

        var items = entries.Select(entry => new AuditLogEntryResponse(
            entry.Id,
            entry.EventType,
            entry.ActorUserId,
            entry.ActorEmail,
            entry.ActorRoles,
            entry.Method,
            entry.Resource,
            entry.StatusCode,
            new DateTimeOffset(DateTime.SpecifyKind(entry.OccurredAt, DateTimeKind.Utc)),
            entry.IpAddress,
            entry.UserAgent,
            entry.DurationMilliseconds,
            entry.EntityName,
            entry.EntityId,
            entry.Details,
            entry.CorrelationId)).ToList();

        return new AuditLogPageResponse(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<List<OverdueKeyDTO>> GetOverdueKeysAsync(
        DateTime now,
        CancellationToken cancellationToken)
    {
        return await _context.KeyTransactions
            .AsNoTracking()
            .Where(transaction =>
                transaction.EndedAt == null &&
                transaction.DueAt != null &&
                transaction.DueAt < now)
            .OrderBy(transaction => transaction.DueAt)
            .Select(transaction => new OverdueKeyDTO
            {
                TransactionId = transaction.Id,
                KeyCode = transaction.Key.Code,
                RoomName = transaction.Key.Room.Name,
                BorrowerUserId = transaction.User.Id,
                BorrowerEmail = transaction.User.Email,
                CheckedOutAt = transaction.StartedAt,
                DueAt = transaction.DueAt!.Value
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TrafficReportDTO> GetTrafficReportAsync(
        string period,
        DateOnly from,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var start = DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var endExclusive = DateTime.SpecifyKind(
            endDate.AddDays(1).ToDateTime(TimeOnly.MinValue),
            DateTimeKind.Utc);

        var keyTransactions = await _context.KeyTransactions
            .AsNoTracking()
            .Where(transaction =>
                (transaction.StartedAt >= start && transaction.StartedAt < endExclusive) ||
                (transaction.EndedAt >= start && transaction.EndedAt < endExclusive))
            .Select(transaction => new
            {
                transaction.StartedAt,
                transaction.EndedAt
            })
            .ToListAsync(cancellationToken);
        var masterKeyTransactions = await _context.MasterKeyTransactions
            .AsNoTracking()
            .Where(transaction =>
                (transaction.StartedAt >= start && transaction.StartedAt < endExclusive) ||
                (transaction.EndedAt >= start && transaction.EndedAt < endExclusive))
            .Select(transaction => new
            {
                transaction.StartedAt,
                transaction.EndedAt
            })
            .ToListAsync(cancellationToken);

        var entries = new SortedDictionary<DateOnly, TrafficReportEntryDTO>();
        var cursor = GetPeriodStart(from, period);
        var lastPeriod = GetPeriodStart(endDate, period);

        while (cursor <= lastPeriod)
        {
            entries.Add(cursor, new TrafficReportEntryDTO { PeriodStart = cursor });
            cursor = period == "weekly" ? cursor.AddDays(7) : cursor.AddDays(1);
        }

        foreach (var transaction in keyTransactions.Concat(masterKeyTransactions))
        {
            if (transaction.StartedAt >= start && transaction.StartedAt < endExclusive)
            {
                entries[GetPeriodStart(DateOnly.FromDateTime(transaction.StartedAt), period)].Checkouts++;
            }

            if (transaction.EndedAt >= start && transaction.EndedAt < endExclusive)
            {
                entries[GetPeriodStart(DateOnly.FromDateTime(transaction.EndedAt!.Value), period)].Returns++;
            }
        }

        return new TrafficReportDTO
        {
            Period = period,
            From = from,
            To = endDate,
            Entries = entries.Values.ToList()
        };
    }

    public async Task<RoomUtilizationReportDTO> GetRoomUtilizationReportAsync(
        string period,
        DateOnly from,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var start = DateTime.SpecifyKind(from.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var endExclusive = DateTime.SpecifyKind(
            endDate.AddDays(1).ToDateTime(TimeOnly.MinValue),
            DateTimeKind.Utc);
        var rooms = await _context.Rooms
            .AsNoTracking()
            .OrderBy(room => room.Building.Name)
            .ThenBy(room => room.Name)
            .Select(room => new
            {
                RoomId = room.Id,
                RoomName = room.Name,
                BuildingName = room.Building.Name
            })
            .ToListAsync(cancellationToken);
        var reservations = await _context.Reservations
            .AsNoTracking()
            .Where(reservation =>
                reservation.StartTime < endExclusive &&
                reservation.EndTime > start)
            .Select(reservation => new
            {
                RoomId = reservation.Room.Id,
                reservation.StartTime,
                reservation.EndTime
            })
            .ToListAsync(cancellationToken);

        var periodStarts = new List<DateOnly>();
        var lastPeriod = GetPeriodStart(endDate, period);
        for (var periodStart = GetPeriodStart(from, period); periodStart <= lastPeriod;)
        {
            periodStarts.Add(periodStart);
            if (periodStart == lastPeriod)
            {
                break;
            }

            periodStart = period == "weekly"
                ? periodStart.AddDays(7)
                : periodStart.AddDays(1);
        }

        var aggregates = new Dictionary<(int RoomId, DateOnly PeriodStart), RoomUtilizationAccumulator>();
        foreach (var reservation in reservations)
        {
            var overlapStart = reservation.StartTime > start ? reservation.StartTime : start;
            var overlapEnd = reservation.EndTime < endExclusive ? reservation.EndTime : endExclusive;
            if (overlapStart >= overlapEnd)
            {
                continue;
            }

            var firstPeriod = GetPeriodStart(DateOnly.FromDateTime(overlapStart), period);
            var lastOverlappingDate = DateOnly.FromDateTime(overlapEnd.AddTicks(-1));
            var lastOverlappingPeriod = GetPeriodStart(lastOverlappingDate, period);
            for (var periodStart = firstPeriod; periodStart <= lastOverlappingPeriod;)
            {
                var periodEnd = DateTime.SpecifyKind(
                    periodStart.AddDays(period == "weekly" ? 7 : 1).ToDateTime(TimeOnly.MinValue),
                    DateTimeKind.Utc);
                var bucketStart = DateTime.SpecifyKind(
                    periodStart.ToDateTime(TimeOnly.MinValue),
                    DateTimeKind.Utc);
                var clippedStart = reservation.StartTime > start ? reservation.StartTime : start;
                if (bucketStart > clippedStart)
                {
                    clippedStart = bucketStart;
                }

                var clippedEnd = reservation.EndTime < endExclusive ? reservation.EndTime : endExclusive;
                if (periodEnd < clippedEnd)
                {
                    clippedEnd = periodEnd;
                }

                var key = (reservation.RoomId, periodStart);
                if (!aggregates.TryGetValue(key, out var aggregate))
                {
                    aggregate = new RoomUtilizationAccumulator();
                    aggregates.Add(key, aggregate);
                }

                aggregate.ReservedHours += (clippedEnd - clippedStart).TotalHours;
                if (reservation.StartTime >= start &&
                    reservation.StartTime < endExclusive &&
                    GetPeriodStart(DateOnly.FromDateTime(reservation.StartTime), period) == periodStart)
                {
                    aggregate.ReservationCount++;
                }

                if (periodStart == lastOverlappingPeriod)
                {
                    break;
                }

                periodStart = period == "weekly"
                    ? periodStart.AddDays(7)
                    : periodStart.AddDays(1);
            }
        }

        var entries = new List<RoomUtilizationEntryDTO>(rooms.Count * periodStarts.Count);
        foreach (var periodStart in periodStarts)
        {
            foreach (var room in rooms)
            {
                aggregates.TryGetValue((room.RoomId, periodStart), out var aggregate);
                entries.Add(new RoomUtilizationEntryDTO(
                    room.RoomId,
                    room.RoomName,
                    room.BuildingName,
                    periodStart,
                    aggregate?.ReservationCount ?? 0,
                    decimal.Round((decimal)(aggregate?.ReservedHours ?? 0), 2)));
            }
        }

        return new RoomUtilizationReportDTO(period, from, endDate, entries);
    }

    private sealed class RoomUtilizationAccumulator
    {
        public int ReservationCount { get; set; }
        public double ReservedHours { get; set; }
    }

    private static DateOnly GetPeriodStart(DateOnly date, string period)
    {
        if (period == "daily")
        {
            return date;
        }

        var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-daysSinceMonday);
    }
}
