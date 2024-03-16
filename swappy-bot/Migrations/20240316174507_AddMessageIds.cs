using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnnouncementIds",
                table: "swap_state",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Replied",
                table: "swap_state",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnouncementIds",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "Replied",
                table: "swap_state");
        }
    }
}
