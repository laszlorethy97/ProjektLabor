using KeyManagement.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyManagement.Api.Migrations;

[DbContext(typeof(KeyManagementDbContext))]
[Migration("20261117085000_SeedDemoAccountsAndData")]
public partial class SeedDemoAccountsAndData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DECLARE @createdAt datetime2(7) = SYSUTCDATETIME();
            DECLARE @sampleStart datetime2(7) = DATEADD(DAY, -2, @createdAt);
            DECLARE @seededRecords TABLE (Resource nvarchar(512) NOT NULL);
            DECLARE @portasId int;
            DECLARE @adminId int;
            DECLARE @directorId int;
            DECLARE @teacherId int;
            DECLARE @buildingId int;
            DECLARE @room101Id int;
            DECLARE @room102Id int;
            DECLARE @projectorId int;
            DECLARE @laptopId int;
            DECLARE @keyId int;
            DECLARE @masterKeyId int;
            DECLARE @reservationId int;

            IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = 1 AND Email = N'lala@lala.com')
            BEGIN
                ;THROW 51000, 'Lala (Users.Id = 1) must exist before demo data is seeded.', 1;
            END;

            IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'portas@portas.com')
            BEGIN
                INSERT INTO Users (Email, PasswordHash, PinCode, CreatedAt, CreatedById)
                OUTPUT CONCAT(N'Users/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'portas@portas.com', N'AQAAAAIAAYagAAAAEDGoFOrG+Ty6KImKazs0lj5W/k9Zm46lyJUZd2wjv5ilbbH9cqImNIfo/g+DkUXB4w==', N'100001', @createdAt, 1);
            END;

            IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'admin@admin.com')
            BEGIN
                INSERT INTO Users (Email, PasswordHash, PinCode, CreatedAt, CreatedById)
                OUTPUT CONCAT(N'Users/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'admin@admin.com', N'AQAAAAIAAYagAAAAEBoEaMT5AxoAvca6r3gbInQdk3TFWETzXJc1Irdz35QlFV+49zOH442qVAKgmvmQ8w==', N'100002', @createdAt, 1);
            END;

            IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'igazgato@igazgato.com')
            BEGIN
                INSERT INTO Users (Email, PasswordHash, PinCode, CreatedAt, CreatedById)
                OUTPUT CONCAT(N'Users/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'igazgato@igazgato.com', N'AQAAAAIAAYagAAAAEKWOXti2pXr9BAVq2smjDRxHC26d592UbOz9Xw2JyReTS9/SpIK5yVGwfiyKYVH5ZA==', N'100003', @createdAt, 1);
            END;

            IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'oktato@oktato.com')
            BEGIN
                INSERT INTO Users (Email, PasswordHash, PinCode, CreatedAt, CreatedById)
                OUTPUT CONCAT(N'Users/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'oktato@oktato.com', N'AQAAAAIAAYagAAAAEO9FabbgUJR8sN7lswmoYR2Zp83GjfSYK7knNm+ykjLcyQBR8+AHbn4S1whFrC2XQg==', N'100004', @createdAt, 1);
            END;

            SELECT @portasId = Id FROM Users WHERE Email = N'portas@portas.com';
            SELECT @adminId = Id FROM Users WHERE Email = N'admin@admin.com';
            SELECT @directorId = Id FROM Users WHERE Email = N'igazgato@igazgato.com';
            SELECT @teacherId = Id FROM Users WHERE Email = N'oktato@oktato.com';

            INSERT INTO UserRoles (CreatedAt, CreatedById, UserId, RoleId)
            OUTPUT CONCAT(N'UserRoles/', inserted.Id) INTO @seededRecords (Resource)
            SELECT @createdAt, 1, account.Id, role.Id
            FROM (VALUES
                (N'portas@portas.com', N'portas'),
                (N'admin@admin.com', N'admin'),
                (N'igazgato@igazgato.com', N'uzemeltetesi-igazgato'),
                (N'oktato@oktato.com', N'oktato')
            ) AS seed(Email, RoleType)
            INNER JOIN Users AS account ON account.Email = seed.Email
            INNER JOIN Roles AS role ON role.Type = seed.RoleType
            WHERE NOT EXISTS (
                SELECT 1
                FROM UserRoles AS existingRole
                WHERE existingRole.UserId = account.Id
                    AND existingRole.RoleId = role.Id
            );

            IF NOT EXISTS (SELECT 1 FROM Buildings WHERE Name = N'Minta A épület')
            BEGIN
                INSERT INTO Buildings (Name)
                OUTPUT CONCAT(N'Buildings/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'Minta A épület');
            END;
            SELECT @buildingId = Id FROM Buildings WHERE Name = N'Minta A épület';

            IF NOT EXISTS (SELECT 1 FROM Rooms WHERE Name = N'Minta 101' AND BuildingId = @buildingId)
            BEGIN
                INSERT INTO Rooms (Name, Capacity, BuildingId)
                OUTPUT CONCAT(N'Rooms/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'Minta 101', 24, @buildingId);
            END;
            SELECT @room101Id = Id FROM Rooms WHERE Name = N'Minta 101' AND BuildingId = @buildingId;

            IF NOT EXISTS (SELECT 1 FROM Rooms WHERE Name = N'Minta 102' AND BuildingId = @buildingId)
            BEGIN
                INSERT INTO Rooms (Name, Capacity, BuildingId)
                OUTPUT CONCAT(N'Rooms/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'Minta 102', 16, @buildingId);
            END;
            SELECT @room102Id = Id FROM Rooms WHERE Name = N'Minta 102' AND BuildingId = @buildingId;

            IF NOT EXISTS (SELECT 1 FROM Equipments WHERE Name = N'Minta projektor' AND RoomId = @room101Id)
            BEGIN
                INSERT INTO Equipments (Name, CreatedAt, CreatedById, RoomId)
                OUTPUT CONCAT(N'Equipments/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'Minta projektor', @createdAt, 1, @room101Id);
            END;
            SELECT @projectorId = Id FROM Equipments WHERE Name = N'Minta projektor' AND RoomId = @room101Id;

            IF NOT EXISTS (SELECT 1 FROM Equipments WHERE Name = N'Minta laptop' AND RoomId = @room101Id)
            BEGIN
                INSERT INTO Equipments (Name, CreatedAt, CreatedById, RoomId)
                OUTPUT CONCAT(N'Equipments/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'Minta laptop', @createdAt, 1, @room101Id);
            END;
            SELECT @laptopId = Id FROM Equipments WHERE Name = N'Minta laptop' AND RoomId = @room101Id;

            IF NOT EXISTS (SELECT 1 FROM Keys WHERE Code = N'SEED-KEY-001')
            BEGIN
                INSERT INTO Keys (Code, CreatedAt, CreatedById, RoomId)
                OUTPUT CONCAT(N'Keys/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'SEED-KEY-001', @createdAt, 1, @room101Id);
            END;
            SELECT @keyId = Id FROM Keys WHERE Code = N'SEED-KEY-001';

            INSERT INTO MasterKeys (CreatedAt, CreatedById)
            OUTPUT CONCAT(N'MasterKeys/', inserted.Id) INTO @seededRecords (Resource)
            VALUES (@createdAt, 1);
            SET @masterKeyId = CONVERT(int, SCOPE_IDENTITY());

            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE UserId = @teacherId AND MasterKeyId = @masterKeyId)
            BEGIN
                INSERT INTO Permissions (CreatedAt, CreatedById, UserId, MasterKeyId)
                OUTPUT CONCAT(N'Permissions/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (@createdAt, 1, @teacherId, @masterKeyId);
            END;

            IF NOT EXISTS (SELECT 1 FROM Reservations WHERE RoomId = @room101Id AND UserId = @teacherId AND StartTime = DATEADD(MINUTE, -5, @sampleStart))
            BEGIN
                INSERT INTO Reservations (StartTime, EndTime, CreatedAt, RoomId, UserId)
                OUTPUT CONCAT(N'Reservations/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (DATEADD(MINUTE, -5, @sampleStart), DATEADD(MINUTE, 40, @sampleStart), @createdAt, @room101Id, @teacherId);
            END;
            SELECT @reservationId = Id
            FROM Reservations
            WHERE RoomId = @room101Id
                AND UserId = @teacherId
                AND StartTime = DATEADD(MINUTE, -5, @sampleStart);

            INSERT INTO EquipmentReservation (EquipmentsId, ReservationsId)
            OUTPUT CONCAT(N'EquipmentReservation/', inserted.EquipmentsId, N'/', inserted.ReservationsId) INTO @seededRecords (Resource)
            SELECT equipment.Id, @reservationId
            FROM Equipments AS equipment
            WHERE equipment.Id IN (@projectorId, @laptopId)
                AND NOT EXISTS (
                    SELECT 1
                    FROM EquipmentReservation AS existingEquipment
                    WHERE existingEquipment.EquipmentsId = equipment.Id
                        AND existingEquipment.ReservationsId = @reservationId
                );

            IF NOT EXISTS (SELECT 1 FROM Maintenances WHERE RoomId = @room102Id AND StartTime = DATEADD(DAY, 7, @createdAt))
            BEGIN
                INSERT INTO Maintenances (StartTime, EndTime, CreatedAt, RoomId, UserId)
                OUTPUT CONCAT(N'Maintenances/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (DATEADD(DAY, 7, @createdAt), DATEADD(DAY, 7, DATEADD(HOUR, 2, @createdAt)), @createdAt, @room102Id, @portasId);
            END;

            IF NOT EXISTS (SELECT 1 FROM Tickets WHERE Description = N'Minta hibajegy: projektor ellenőrzése')
            BEGIN
                INSERT INTO Tickets (Description, CreatedAt, CreatedById)
                OUTPUT CONCAT(N'Tickets/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'Minta hibajegy: projektor ellenőrzése', @createdAt, 1);
            END;

            IF NOT EXISTS (SELECT 1 FROM KeyTransactions WHERE KeyId = @keyId AND UserId = @teacherId AND StartedAt = @sampleStart)
            BEGIN
                INSERT INTO KeyTransactions (PinCode, StartedAt, DueAt, EndedAt, StartedByUserId, EndedByUserId, UserId, KeyId)
                OUTPUT CONCAT(N'KeyTransactions/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'100004', @sampleStart, DATEADD(MINUTE, 40, @sampleStart), DATEADD(MINUTE, 30, @sampleStart), @portasId, @portasId, @teacherId, @keyId);
            END;

            IF NOT EXISTS (SELECT 1 FROM MasterKeyTransactions WHERE MasterKeyId = @masterKeyId AND UserId = @teacherId AND StartedAt = @sampleStart)
            BEGIN
                INSERT INTO MasterKeyTransactions (PinCode, StartedAt, EndedAt, StartedByUserId, EndedByUserId, ApprovedByUserId, UserId, MasterKeyId)
                OUTPUT CONCAT(N'MasterKeyTransactions/', inserted.Id) INTO @seededRecords (Resource)
                VALUES (N'100004', @sampleStart, DATEADD(MINUTE, 30, @sampleStart), @portasId, @portasId, @adminId, @teacherId, @masterKeyId);
            END;

            INSERT INTO AuditLogs (ActorUserId, Method, Resource, StatusCode, OccurredAt, IpAddress, UserAgent, DurationMilliseconds)
            SELECT 1, N'SEED', Resource, 201, @createdAt, NULL, N'startup migration', 0
            FROM @seededRecords;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Keep seeded records on downgrade because they may already be in use.
    }
}
