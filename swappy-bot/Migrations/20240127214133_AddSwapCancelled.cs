using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class AddSwapCancelled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SwapCancelled",
                table: "swap_state",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_swap_state_SwapCancelled",
                table: "swap_state",
                column: "SwapCancelled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_swap_state_SwapCancelled",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "SwapCancelled",
                table: "swap_state");
        }
    }
}
