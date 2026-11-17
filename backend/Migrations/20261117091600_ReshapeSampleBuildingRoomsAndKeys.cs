using KeyManagement.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyManagement.Api.Migrations;

[DbContext(typeof(KeyManagementDbContext))]
[Migration("20261117091600_ReshapeSampleBuildingRoomsAndKeys")]
public partial class ReshapeSampleBuildingRoomsAndKeys : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DECLARE @createdAt datetime2(7) = SYSUTCDATETIME();
            DECLARE @seededRecords TABLE (Method nvarchar(10) NOT NULL, Resource nvarchar(512) NOT NULL);
            DECLARE @buildingAId int;
            DECLARE @buildingBId int;
            DECLARE @buildingIId int;

            IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = 1 AND Email = N'lala@lala.com')
            BEGIN
                ;THROW 51000, 'Lala (Users.Id = 1) must exist before sample data is reshaped.', 1;
            END;

            IF NOT EXISTS (SELECT 1 FROM Buildings WHERE Name = N'A épület')
                AND EXISTS (SELECT 1 FROM Buildings WHERE Name = N'Minta A épület')
            BEGIN
                UPDATE Buildings
                SET Name = N'A épület'
                OUTPUT N'UPDATE', CONCAT(N'Buildings/', inserted.Id)
                    INTO @seededRecords (Method, Resource)
                WHERE Name = N'Minta A épület';
            END;

            IF NOT EXISTS (SELECT 1 FROM Buildings WHERE Name = N'A épület')
            BEGIN
                INSERT INTO Buildings (Name)
                OUTPUT N'SEED', CONCAT(N'Buildings/', inserted.Id)
                    INTO @seededRecords (Method, Resource)
                VALUES (N'A épület');
            END;
            SELECT TOP (1) @buildingAId = Id FROM Buildings WHERE Name = N'A épület' ORDER BY Id;

            IF NOT EXISTS (SELECT 1 FROM Buildings WHERE Name = N'B épület')
            BEGIN
                INSERT INTO Buildings (Name)
                OUTPUT N'SEED', CONCAT(N'Buildings/', inserted.Id)
                    INTO @seededRecords (Method, Resource)
                VALUES (N'B épület');
            END;
            SELECT TOP (1) @buildingBId = Id FROM Buildings WHERE Name = N'B épület' ORDER BY Id;

            IF NOT EXISTS (SELECT 1 FROM Buildings WHERE Name = N'I épület')
            BEGIN
                INSERT INTO Buildings (Name)
                OUTPUT N'SEED', CONCAT(N'Buildings/', inserted.Id)
                    INTO @seededRecords (Method, Resource)
                VALUES (N'I épület');
            END;
            SELECT TOP (1) @buildingIId = Id FROM Buildings WHERE Name = N'I épület' ORDER BY Id;

            IF EXISTS (
                SELECT 1 FROM Rooms
                WHERE Name = N'Minta 101' AND BuildingId = @buildingAId
            ) AND NOT EXISTS (
                SELECT 1 FROM Rooms
                WHERE Name = N'A1' AND BuildingId = @buildingAId
            )
            BEGIN
                UPDATE Rooms
                SET Name = N'A1'
                OUTPUT N'UPDATE', CONCAT(N'Rooms/', inserted.Id)
                    INTO @seededRecords (Method, Resource)
                WHERE Name = N'Minta 101' AND BuildingId = @buildingAId;
            END;

            IF EXISTS (
                SELECT 1 FROM Rooms
                WHERE Name = N'Minta 102' AND BuildingId = @buildingAId
            ) AND NOT EXISTS (
                SELECT 1 FROM Rooms
                WHERE Name = N'A2' AND BuildingId = @buildingAId
            )
            BEGIN
                UPDATE Rooms
                SET Name = N'A2'
                OUTPUT N'UPDATE', CONCAT(N'Rooms/', inserted.Id)
                    INTO @seededRecords (Method, Resource)
                WHERE Name = N'Minta 102' AND BuildingId = @buildingAId;
            END;

            INSERT INTO Rooms (Name, Capacity, BuildingId)
            OUTPUT N'SEED', CONCAT(N'Rooms/', inserted.Id)
                INTO @seededRecords (Method, Resource)
            SELECT sample.RoomName, 24, building.Id
            FROM (VALUES
                (N'A1', N'A épület'),
                (N'A2', N'A épület'),
                (N'A3', N'A épület'),
                (N'B1', N'B épület'),
                (N'B2', N'B épület'),
                (N'B3', N'B épület'),
                (N'I1', N'I épület'),
                (N'I2', N'I épület'),
                (N'I3', N'I épület')
            ) AS sample(RoomName, BuildingName)
            INNER JOIN Buildings AS building ON building.Name = sample.BuildingName
            WHERE NOT EXISTS (
                SELECT 1
                FROM Rooms AS existingRoom
                WHERE existingRoom.Name = sample.RoomName
                    AND existingRoom.BuildingId = building.Id
            );

            IF EXISTS (
                SELECT 1
                FROM Keys AS sampleKey
                INNER JOIN Rooms AS room ON room.Id = sampleKey.RoomId
                WHERE sampleKey.Code = N'SEED-KEY-001'
                    AND room.Name = N'A1'
                    AND room.BuildingId = @buildingAId
            ) AND NOT EXISTS (
                SELECT 1 FROM Keys WHERE Code = N'A1/1'
            )
            BEGIN
                UPDATE Keys
                SET Code = N'A1/1'
                OUTPUT N'UPDATE', CONCAT(N'Keys/', inserted.Id)
                    INTO @seededRecords (Method, Resource)
                WHERE Code = N'SEED-KEY-001';
            END;

            INSERT INTO Keys (Code, CreatedAt, CreatedById, RoomId)
            OUTPUT N'SEED', CONCAT(N'Keys/', inserted.Id)
                INTO @seededRecords (Method, Resource)
            SELECT sample.KeyCode, @createdAt, 1, room.Id
            FROM (VALUES
                (N'A1', N'A1/1'), (N'A1', N'A1/2'),
                (N'A2', N'A2/1'), (N'A2', N'A2/2'),
                (N'A3', N'A3/1'), (N'A3', N'A3/2'),
                (N'B1', N'B1/1'), (N'B1', N'B1/2'),
                (N'B2', N'B2/1'), (N'B2', N'B2/2'),
                (N'B3', N'B3/1'), (N'B3', N'B3/2'),
                (N'I1', N'I1/1'), (N'I1', N'I1/2'),
                (N'I2', N'I2/1'), (N'I2', N'I2/2'),
                (N'I3', N'I3/1'), (N'I3', N'I3/2')
            ) AS sample(RoomName, KeyCode)
            INNER JOIN Rooms AS room ON room.Name = sample.RoomName
            INNER JOIN Buildings AS building ON building.Id = room.BuildingId
            WHERE (
                    (sample.RoomName LIKE N'A%' AND building.Id = @buildingAId) OR
                    (sample.RoomName LIKE N'B%' AND building.Id = @buildingBId) OR
                    (sample.RoomName LIKE N'I%' AND building.Id = @buildingIId)
                )
                AND NOT EXISTS (
                    SELECT 1 FROM Keys AS existingKey WHERE existingKey.Code = sample.KeyCode
                );

            INSERT INTO AuditLogs (ActorUserId, Method, Resource, StatusCode, OccurredAt, IpAddress, UserAgent, DurationMilliseconds)
            SELECT 1, Method, Resource, 200, @createdAt, NULL, N'sample data migration', 0
            FROM @seededRecords;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Keep the sample records on downgrade because they may already be in use.
    }
}
