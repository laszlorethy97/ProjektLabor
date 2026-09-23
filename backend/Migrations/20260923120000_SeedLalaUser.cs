using System;
using KeyManagement.Api.Data;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace KeyManagement.Api.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(KeyManagementDbContext))]
    [Migration("20260923120000_SeedLalaUser")]
    public partial class SeedLalaUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SET IDENTITY_INSERT Roles ON;
                INSERT INTO Roles (Id, Type)
                VALUES
                    (1, N'oktato'),
                    (2, N'portas'),
                    (3, N'admin'),
                    (4, N'uzemeltetesi-igazgato');
                SET IDENTITY_INSERT Roles OFF;
                """);

            migrationBuilder.Sql("""
                SET IDENTITY_INSERT Users ON;
                INSERT INTO Users (Id, Email, PasswordHash, PinCode, CreatedAt, CreatedById)
                VALUES (1, N'lala@lala.com', N'AQIAAACghgEAEAAAAL6x+JNkhuoePm0WlApJuc8AzOnBZ26ee3OlpAeRP4OSDmd3m2BLOUwEraWhHi09kg==', N'000000', '2026-09-23T00:00:00.0000000Z', NULL);
                SET IDENTITY_INSERT Users OFF;
                """);

            migrationBuilder.Sql("""
                SET IDENTITY_INSERT UserRoles ON;
                INSERT INTO UserRoles (Id, CreatedAt, CreatedById, UserId, RoleId)
                VALUES (1, '2026-09-23T00:00:00.0000000Z', 1, 1, 1);
                SET IDENTITY_INSERT UserRoles OFF;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM UserRoles WHERE Id = 1;");
            migrationBuilder.Sql("DELETE FROM Users WHERE Id = 1;");
            migrationBuilder.Sql("DELETE FROM Roles WHERE Id = 1;");
        }
    }
}
