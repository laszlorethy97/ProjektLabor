using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class UseShortBuildingNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DECLARE @occurredAt datetime2(7) = SYSUTCDATETIME();
                DECLARE @updatedBuildings TABLE (Id int NOT NULL);

                UPDATE Buildings
                SET Name = CASE Name
                    WHEN N'A épület' THEN N'A'
                    WHEN N'B épület' THEN N'B'
                    WHEN N'I épület' THEN N'I'
                END
                OUTPUT inserted.Id INTO @updatedBuildings (Id)
                WHERE Name IN (N'A épület', N'B épület', N'I épület');

                INSERT INTO AuditLogs (
                    ActorUserId, Method, Resource, StatusCode, OccurredAt,
                    IpAddress, UserAgent, DurationMilliseconds)
                SELECT 1, N'UPDATE', CONCAT(N'Buildings/', Id), 200, @occurredAt,
                    NULL, N'data migration', 0
                FROM @updatedBuildings;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Buildings
                SET Name = CASE Name
                    WHEN N'A' THEN N'A épület'
                    WHEN N'B' THEN N'B épület'
                    WHEN N'I' THEN N'I épület'
                END
                WHERE Name IN (N'A', N'B', N'I');
                """);
        }
    }
}
