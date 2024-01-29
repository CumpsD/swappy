using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class AddDepositFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepositAddress",
                table: "swap_state",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DepositChannel",
                table: "swap_state",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DepositGenerated",
                table: "swap_state",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_swap_state_DepositGenerated",
                table: "swap_state",
                column: "DepositGenerated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_swap_state_DepositGenerated",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "DepositAddress",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "DepositChannel",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "DepositGenerated",
                table: "swap_state");
        }
    }
}
