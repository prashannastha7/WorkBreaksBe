using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class StartDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1,
                column: "Password",
                value: "$2a$11$mRShmYcY4bEP8SM1dzM5UepugL/6qBhxyx3ZtTIglO9LkM1wiDKlG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1,
                column: "Password",
                value: "$2a$11$/6xsF82Fl7hpPZMFRq6vFuOe3BdjEkHnw5XOLvpiRN11u8n2fqnNO");
        }
    }
}
