using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "refundaddress",
                table: "swap_state",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refundaddress",
                table: "swap_state");
        }
    }
}
