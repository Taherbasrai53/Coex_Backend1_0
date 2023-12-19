using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COeX_India1._0.Migrations
{
    /// <inheritdoc />
    public partial class liveRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllocationStatus",
                table: "Mines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LiveRequests",
                table: "Clusters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllocationStatus",
                table: "Mines");

            migrationBuilder.DropColumn(
                name: "LiveRequests",
                table: "Clusters");
        }
    }
}
