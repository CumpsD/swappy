using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "quotetype",
                table: "swap_state",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quotetype",
                table: "swap_state");
        }
    }
}
