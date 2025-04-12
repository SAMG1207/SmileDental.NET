using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmileDental.Migrations
{
    /// <inheritdoc />
    public partial class AL2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ActionLoges",
                table: "ActionLoges");

            migrationBuilder.RenameTable(
                name: "ActionLoges",
                newName: "Logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logs",
                table: "Logs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Logs",
                table: "Logs");

            migrationBuilder.RenameTable(
                name: "Logs",
                newName: "ActionLoges");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActionLoges",
                table: "ActionLoges",
                column: "Id");
        }
    }
}
