using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmileDental.Migrations
{
    /// <inheritdoc />
    public partial class _2303 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Dentistas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Dentistas");
        }
    }
}
