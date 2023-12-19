using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COeX_India1._0.Migrations
{
    /// <inheritdoc />
    public partial class TentativelyAvailable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TentativelyAvailable",
                table: "Clusters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TentativelyAvailable",
                table: "Clusters");
        }
    }
}
