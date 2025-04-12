using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmileDental.Migrations
{
    /// <inheritdoc />
    public partial class ActionLog : Migration
    {
        private int idUser;
        private string role;
        private string accion;
        private int responseCode;
        private string responseMessage;

        public ActionLog(int IdUser, string role, string accion, int responseCode, string responseMessage)
        {
            idUser = IdUser;
            this.role = role;
            this.accion = accion;
            this.responseCode = responseCode;
            this.responseMessage = responseMessage;
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "URLCita",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
