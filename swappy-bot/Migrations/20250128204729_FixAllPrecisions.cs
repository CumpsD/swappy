using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwappyBot.Migrations
{
    /// <inheritdoc />
    public partial class FixAllPrecisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "quotereceive",
                table: "swap_state",
                type: "numeric(27,18)",
                precision: 27,
                scale: 18,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "quoteplatformfee",
                table: "swap_state",
                type: "numeric(27,18)",
                precision: 27,
                scale: 18,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "quotedeposit",
                table: "swap_state",
                type: "numeric(27,18)",
                precision: 27,
                scale: 18,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "quotechainflipfee",
                table: "swap_state",
                type: "numeric(27,18)",
                precision: 27,
                scale: 18,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "quotereceive",
                table: "swap_state",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(27,18)",
                oldPrecision: 27,
                oldScale: 18,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "quoteplatformfee",
                table: "swap_state",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(27,18)",
                oldPrecision: 27,
                oldScale: 18,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "quotedeposit",
                table: "swap_state",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(27,18)",
                oldPrecision: 27,
                oldScale: 18,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "quotechainflipfee",
                table: "swap_state",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(27,18)",
                oldPrecision: 27,
                oldScale: 18,
                oldNullable: true);
        }
    }
}
