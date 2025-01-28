using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "swap_state",
                columns: table => new
                {
                    stateid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    swapstarted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    assetfrom = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    assetto = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    amount = table.Column<decimal>(type: "numeric", nullable: true),
                    destinationaddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    quotetime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    quotedeposit = table.Column<decimal>(type: "numeric", nullable: true),
                    quotereceive = table.Column<decimal>(type: "numeric", nullable: true),
                    quoterate = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    quoteplatformfee = table.Column<decimal>(type: "numeric", nullable: true),
                    quotechainflipfee = table.Column<decimal>(type: "numeric", nullable: true),
                    swapaccepted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    swapcancelled = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    depositgenerated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    depositaddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    depositchannel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    swapstatus = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    announcementids = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    replied = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_swap_state", x => x.stateid);
                });

            migrationBuilder.CreateIndex(
                name: "ix_swap_state_depositgenerated",
                table: "swap_state",
                column: "depositgenerated");

            migrationBuilder.CreateIndex(
                name: "ix_swap_state_quotetime",
                table: "swap_state",
                column: "quotetime");

            migrationBuilder.CreateIndex(
                name: "ix_swap_state_swapaccepted",
                table: "swap_state",
                column: "swapaccepted");

            migrationBuilder.CreateIndex(
                name: "ix_swap_state_swapcancelled",
                table: "swap_state",
                column: "swapcancelled");

            migrationBuilder.CreateIndex(
                name: "ix_swap_state_swapstarted",
                table: "swap_state",
                column: "swapstarted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "swap_state");
        }
    }
}
