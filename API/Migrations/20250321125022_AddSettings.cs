using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Setting",
                columns: new[] { "SettingID", "CreatedDate", "DataType", "ModifiedDate", "SettingKey", "SettingValue" },
                values: new object[,]
                {
                    { new Guid("3ca30af9-1538-4339-91d5-cd90a8ef44e4"), null, 1, null, "SocialMediaUrls", "[]" },
                    { new Guid("4e5050c5-1a3c-482f-8169-e902fcca464f"), null, 1, null, "RestaurantPhoneNumber", "" },
                    { new Guid("69a4356b-1f0a-40d5-ab4d-a75a2f0e16fb"), null, 1, null, "OpeningTimes", "[]" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "SettingID",
                keyValue: new Guid("3ca30af9-1538-4339-91d5-cd90a8ef44e4"));

            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "SettingID",
                keyValue: new Guid("4e5050c5-1a3c-482f-8169-e902fcca464f"));

            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "SettingID",
                keyValue: new Guid("69a4356b-1f0a-40d5-ab4d-a75a2f0e16fb"));
        }
    }
}
