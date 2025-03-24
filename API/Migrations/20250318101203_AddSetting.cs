using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Setting",
                columns: new[] { "SettingID", "CreatedDate", "DataType", "ModifiedDate", "SettingKey", "SettingValue" },
                values: new object[] { new Guid("c5bfe361-32f5-4b12-ba50-fc877e88c1f9"), null, 1, null, "ListMenuScreenForCustomerImages", "[]" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "SettingID",
                keyValue: new Guid("c5bfe361-32f5-4b12-ba50-fc877e88c1f9"));
        }
    }
}
