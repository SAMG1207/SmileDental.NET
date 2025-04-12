using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmileDental.Migrations
{
    /// <inheritdoc />
    public partial class _11abr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponseCode",
                table: "Logs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ResponseMessage",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "URLCita",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseCode",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ResponseMessage",
                table: "Logs");

            migrationBuilder.AlterColumn<string>(
                name: "URLCita",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
