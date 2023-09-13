using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MassTransitPlay.Stats.Domain.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeToTotals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Totals",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Totals_Type",
                table: "Totals",
                column: "Type",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Totals_Type",
                table: "Totals");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Totals");
        }
    }
}
