using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KeyManagement.Api.ServiceTests;

public class AuditLogServiceTest
{
    [Fact]
    public async Task GetOverdueKeysAsync_ReturnsOnlyOpenTransactionsPastTheirDueDate()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);

        var now = new DateTime(2026, 11, 17, 12, 0, 0, DateTimeKind.Utc);
        var user = new User { Id = 1, Email = "borrower@example.com" };
        var room = new Room { Id = 1, Name = "Room 1" };
        var key = new Key { Id = 1, Code = "KEY-001", Room = room };
        context.KeyTransactions.AddRange(
            new KeyTransaction
            {
                Id = 1,
                Key = key,
                User = user,
                StartedByUser = user,
                StartedAt = now.AddHours(-2),
                DueAt = now.AddMinutes(-1)
            },
            new KeyTransaction
            {
                Id = 2,
                Key = key,
                User = user,
                StartedByUser = user,
                StartedAt = now.AddHours(-1),
                DueAt = now.AddMinutes(1)
            },
            new KeyTransaction
            {
                Id = 3,
                Key = key,
                User = user,
                StartedByUser = user,
                StartedAt = now.AddHours(-3),
                DueAt = now.AddHours(-1),
                EndedAt = now.AddMinutes(-30)
            });
        await context.SaveChangesAsync();

        var service = new AuditLogService(context);

        var overdueKeys = await service.GetOverdueKeysAsync(now, CancellationToken.None);

        var overdueKey = Assert.Single(overdueKeys);
        Assert.Equal(1, overdueKey.TransactionId);
        Assert.Equal("KEY-001", overdueKey.KeyCode);
        Assert.Equal(user.Email, overdueKey.BorrowerEmail);
    }

    [Fact]
    public async Task GetTrafficReportAsync_AggregatesCheckoutsAndReturnsByRequestedPeriod()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);

        var monday = new DateTime(2026, 11, 16, 9, 0, 0, DateTimeKind.Utc);
        var user = new User { Id = 1, Email = "borrower@example.com" };
        var room = new Room { Id = 1, Name = "Room 1" };
        var key = new Key { Id = 1, Code = "KEY-001", Room = room };
        context.KeyTransactions.Add(new KeyTransaction
        {
            Id = 1,
            Key = key,
            User = user,
            StartedByUser = user,
            StartedAt = monday,
            EndedAt = monday.AddDays(2)
        });
        context.MasterKeyTransactions.Add(new MasterKeyTransaction
        {
            Id = 2,
            MasterKey = new MasterKey { Id = 1, CreatedBy = user },
            User = user,
            StartedByUser = user,
            ApprovedByUser = user,
            StartedAt = monday.AddDays(1),
            EndedAt = monday.AddDays(3)
        });
        await context.SaveChangesAsync();

        var service = new AuditLogService(context);
        var from = DateOnly.FromDateTime(monday);
        var to = from.AddDays(6);

        var dailyReport = await service.GetTrafficReportAsync(
            "daily",
            from,
            to,
            CancellationToken.None);
        var weeklyReport = await service.GetTrafficReportAsync(
            "weekly",
            from,
            to,
            CancellationToken.None);

        Assert.Equal(7, dailyReport.Entries.Count);
        Assert.Equal(1, dailyReport.Entries[0].Checkouts);
        Assert.Equal(1, dailyReport.Entries[1].Checkouts);
        Assert.Equal(1, dailyReport.Entries[2].Returns);
        Assert.Equal(1, dailyReport.Entries[3].Returns);
        var weeklyEntry = Assert.Single(weeklyReport.Entries);
        Assert.Equal(from, weeklyEntry.PeriodStart);
        Assert.Equal(2, weeklyEntry.Checkouts);
        Assert.Equal(2, weeklyEntry.Returns);
    }

    [Fact]
    public async Task GetRoomUtilizationReportAsync_AggregatesReservationsAndHoursByRoomAndPeriod()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);

        var monday = new DateTime(2026, 11, 16, 9, 0, 0, DateTimeKind.Utc);
        var user = new User { Id = 1, Email = "borrower@example.com" };
        var building = new Building { Id = 1, Name = "A" };
        var firstRoom = new Room { Id = 1, Name = "A1", Building = building };
        var secondRoom = new Room { Id = 2, Name = "A2", Building = building };
        context.Rooms.AddRange(firstRoom, secondRoom);
        context.Reservations.AddRange(
            new Reservation
            {
                Id = 1,
                Room = firstRoom,
                User = user,
                StartTime = monday,
                EndTime = monday.AddMinutes(45)
            },
            new Reservation
            {
                Id = 2,
                Room = secondRoom,
                User = user,
                StartTime = monday.AddDays(1).AddMinutes(15),
                EndTime = monday.AddDays(1).AddHours(1)
            });
        await context.SaveChangesAsync();

        var service = new AuditLogService(context);
        var from = DateOnly.FromDateTime(monday);
        var to = from.AddDays(1);
        var dailyReport = await service.GetRoomUtilizationReportAsync(
            "daily",
            from,
            to,
            CancellationToken.None);
        var weeklyReport = await service.GetRoomUtilizationReportAsync(
            "weekly",
            from,
            to,
            CancellationToken.None);

        Assert.Equal(4, dailyReport.Entries.Count);
        var firstRoomMonday = Assert.Single(dailyReport.Entries, entry =>
            entry.RoomId == firstRoom.Id && entry.PeriodStart == from);
        Assert.Equal(1, firstRoomMonday.ReservationCount);
        Assert.Equal(0.75m, firstRoomMonday.ReservedHours);
        var firstRoomTuesday = Assert.Single(dailyReport.Entries, entry =>
            entry.RoomId == firstRoom.Id && entry.PeriodStart == to);
        Assert.Equal(0, firstRoomTuesday.ReservationCount);
        Assert.Equal(0m, firstRoomTuesday.ReservedHours);
        var secondRoomTuesday = Assert.Single(dailyReport.Entries, entry =>
            entry.RoomId == secondRoom.Id && entry.PeriodStart == to);
        Assert.Equal(1, secondRoomTuesday.ReservationCount);
        Assert.Equal(0.75m, secondRoomTuesday.ReservedHours);

        Assert.Equal(2, weeklyReport.Entries.Count);
        Assert.All(weeklyReport.Entries, entry =>
        {
            Assert.Equal(1, entry.ReservationCount);
            Assert.Equal(0.75m, entry.ReservedHours);
        });
    }

    [Fact]
    public async Task GetEventsAsyncFiltersAndPagesEventsInStableNewestFirstOrder()
    {
        var options = new DbContextOptionsBuilder<KeyManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new KeyManagementDbContext(options);
        var occurredAt = new DateTime(2026, 11, 17, 12, 0, 0, DateTimeKind.Utc);
        context.AuditLogs.AddRange(
            new AuditLog
            {
                Id = 1,
                EventType = AuditLogEventTypes.DataChange,
                ActorUserId = 2,
                ActorEmail = "operator@example.com",
                Method = "UPDATE",
                Resource = "Room/{\"Id\":1}",
                EntityName = "Room",
                OccurredAt = occurredAt
            },
            new AuditLog
            {
                Id = 2,
                EventType = AuditLogEventTypes.DataChange,
                ActorUserId = 2,
                ActorEmail = "operator@example.com",
                Method = "UPDATE",
                Resource = "Room/{\"Id\":2}",
                EntityName = "Room",
                OccurredAt = occurredAt
            },
            new AuditLog
            {
                Id = 3,
                EventType = AuditLogEventTypes.HttpRequest,
                Method = "GET",
                Resource = "/api/Room",
                OccurredAt = occurredAt
            });
        await context.SaveChangesAsync();

        var service = new AuditLogService(context);
        var result = await service.GetEventsAsync(
            new AuditLogQuery
            {
                EventType = AuditLogEventTypes.DataChange,
                Search = "operator@example.com",
                Page = 2,
                PageSize = 1
            },
            CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Page);
        var entry = Assert.Single(result.Items);
        Assert.Equal(1, entry.Id);
        Assert.Equal("operator@example.com", entry.ActorEmail);
    }
}
