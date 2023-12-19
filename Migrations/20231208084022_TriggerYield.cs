using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COeX_India1._0.Migrations
{
    /// <inheritdoc />
    public partial class TriggerYield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Trigger",
                table: "Mines",
                newName: "TriggerYield");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TriggerYield",
                table: "Mines",
                newName: "Trigger");
        }
    }
}
