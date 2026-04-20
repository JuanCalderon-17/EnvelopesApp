using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cdpTracker_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBcryptAndKiosks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$iE/mpYk0zTDDKPDtfhxPyOZ2lFfxnlAGP/sn0p.yR3PQZtG7e3OS2");

            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$Qhy6d5hyPmjk5kIKhJ8kSek.0wObNSSsHzE6B83DFnnNW6R1cN9sm");

            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$i8X0D4..nZMXolIfRO9RZOS9NZUclVLIZydpOjlej.S7D2m5fUnU.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "kiosko2");

            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "kiosko3");

            migrationBuilder.UpdateData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "kiosko5");
        }
    }
}
