using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cdpTracker_Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameWorkersToKiosks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "PasswordHash", "Role" },
                values: new object[] { "Kiosko 2", "kiosko2", 0 });

            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "PasswordHash" },
                values: new object[] { "Kiosko 3", "kiosko3" });

            migrationBuilder.InsertData(
                table: "Workers",
                columns: new[] { "Id", "Kiosko", "Name", "PasswordHash", "Role" },
                values: new object[] { 3, 2, "Kiosko 5", "kiosko5", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "PasswordHash", "Role" },
                values: new object[] { "Admin Manager", "hashed_admin_pass", 1 });

            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "PasswordHash" },
                values: new object[] { "Juan Calderon", "hashed_john_pass" });
        }
    }
}
