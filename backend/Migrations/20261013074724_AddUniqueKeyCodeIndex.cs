using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueKeyCodeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Keys_Code",
                table: "Keys",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Keys_Code",
                table: "Keys");
        }
    }
}
