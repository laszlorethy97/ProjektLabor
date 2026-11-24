using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogAndKeyDueAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueAt",
                table: "KeyTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(
                """
                EXEC sp_executesql N'
                UPDATE kt
                SET DueAt = (
                    SELECT TOP (1) r.EndTime
                    FROM Reservations AS r
                    INNER JOIN Keys AS k ON k.RoomId = r.RoomId
                    WHERE k.Id = kt.KeyId
                        AND r.UserId = kt.UserId
                        AND r.StartTime <= DATEADD(MINUTE, 10, kt.StartedAt)
                        AND r.EndTime >= kt.StartedAt
                    ORDER BY ABS(DATEDIFF_BIG(SECOND, r.StartTime, kt.StartedAt))
                )
                FROM KeyTransactions AS kt
                WHERE EXISTS (
                    SELECT 1
                    FROM Reservations AS r
                    INNER JOIN Keys AS k ON k.RoomId = r.RoomId
                    WHERE k.Id = kt.KeyId
                        AND r.UserId = kt.UserId
                        AND r.StartTime <= DATEADD(MINUTE, 10, kt.StartedAt)
                        AND r.EndTime >= kt.StartedAt
                );
                ';
                """);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActorUserId = table.Column<int>(type: "int", nullable: true),
                    Method = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DurationMilliseconds = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OccurredAt",
                table: "AuditLogs",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_KeyTransactions_EndedAt_DueAt",
                table: "KeyTransactions",
                columns: new[] { "EndedAt", "DueAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_KeyTransactions_EndedAt_DueAt",
                table: "KeyTransactions");

            migrationBuilder.DropColumn(
                name: "DueAt",
                table: "KeyTransactions");
        }
    }
}
