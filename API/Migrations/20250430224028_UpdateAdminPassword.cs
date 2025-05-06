using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "UserLoginID",
                keyValue: new Guid("8b59dd9f-72d8-4d01-a971-03bc98c2262f"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEKqe/dcgGzIC8jgfJymczuCBpLYck9TdfxOQ19M6h9o4qBbTxjCk8PP2fzb49fPGPQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserLogin",
                keyColumn: "UserLoginID",
                keyValue: new Guid("8b59dd9f-72d8-4d01-a971-03bc98c2262f"),
                column: "Password",
                value: "123456");
        }
    }
}
