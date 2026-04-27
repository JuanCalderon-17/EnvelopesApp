using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cdpTracker_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorNameAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "Envelopes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Envelopes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "Envelopes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Envelopes");
        }
    }
}
