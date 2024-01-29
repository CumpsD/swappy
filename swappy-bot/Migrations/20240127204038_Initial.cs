using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "swap_state",
                columns: table => new
                {
                    StateId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SwapStarted = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ServerId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ServerName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    UserName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssetFrom = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssetTo = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<double>(type: "double", nullable: true),
                    DestinationAddress = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuoteTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    QuoteDeposit = table.Column<double>(type: "double", nullable: true),
                    QuoteReceive = table.Column<double>(type: "double", nullable: true),
                    QuoteRate = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuotePlatformFee = table.Column<double>(type: "double", nullable: true),
                    QuoteChainflipFee = table.Column<double>(type: "double", nullable: true),
                    SwapAccepted = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swap_state", x => x.StateId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_swap_state_QuoteTime",
                table: "swap_state",
                column: "QuoteTime");

            migrationBuilder.CreateIndex(
                name: "IX_swap_state_ServerId",
                table: "swap_state",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_swap_state_SwapAccepted",
                table: "swap_state",
                column: "SwapAccepted");

            migrationBuilder.CreateIndex(
                name: "IX_swap_state_SwapStarted",
                table: "swap_state",
                column: "SwapStarted");

            migrationBuilder.CreateIndex(
                name: "IX_swap_state_UserId",
                table: "swap_state",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "swap_state");
        }
    }
}
