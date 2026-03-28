using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace cdpTracker_Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialWorkers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Workers",
                columns: new[] { "Id", "Kiosko", "Name", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, 0, "Admin Manager", "hashed_admin_pass", 1 },
                    { 2, 1, "Juan Calderon", "hashed_john_pass", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
