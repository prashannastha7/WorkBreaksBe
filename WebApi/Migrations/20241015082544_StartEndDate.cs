using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class StartEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1,
                column: "Password",
                value: "$2a$11$JtSNCtCjc1NYcaw4EZfhl.nnWyVNiS6d4LLcTIUibmlRABwRaKcey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1,
                column: "Password",
                value: "$2a$11$mRShmYcY4bEP8SM1dzM5UepugL/6qBhxyx3ZtTIglO9LkM1wiDKlG");
        }
    }
}
