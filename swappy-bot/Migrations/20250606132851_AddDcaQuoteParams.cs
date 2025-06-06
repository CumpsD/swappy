using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class AddDcaQuoteParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "quotechunkinterval",
                table: "swap_state",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "quotenumberofchunks",
                table: "swap_state",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quotechunkinterval",
                table: "swap_state");

            migrationBuilder.DropColumn(
                name: "quotenumberofchunks",
                table: "swap_state");
        }
    }
}
