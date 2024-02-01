using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_swap_state_ServerId",
                table: "swap_state");

            migrationBuilder.DropIndex(
                name: "IX_swap_state_UserId",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "ServerName",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "swap_state");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "ServerId",
                table: "swap_state",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<string>(
                name: "ServerName",
                table: "swap_state",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<ulong>(
                name: "UserId",
                table: "swap_state",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "swap_state",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_swap_state_ServerId",
                table: "swap_state",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_swap_state_UserId",
                table: "swap_state",
                column: "UserId");
        }
    }
}
