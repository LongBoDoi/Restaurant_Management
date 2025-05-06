using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddManageSettingPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "PermissionID", "CreatedDate", "ModifiedDate", "PermissionCode", "PermissionName" },
                values: new object[] { new Guid("9faeb1ff-0e3f-4faf-afa4-80b0e90b4f60"), null, null, "ManageSetting", "Quản lý thiết lập" });

            migrationBuilder.InsertData(
                table: "PermissionRole",
                columns: new[] { "PermissionsPermissionID", "RolesRoleID" },
                values: new object[,]
                {
                    { new Guid("9faeb1ff-0e3f-4faf-afa4-80b0e90b4f60"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") },
                    { new Guid("9faeb1ff-0e3f-4faf-afa4-80b0e90b4f60"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermissionRole",
                keyColumns: new[] { "PermissionsPermissionID", "RolesRoleID" },
                keyValues: new object[] { new Guid("9faeb1ff-0e3f-4faf-afa4-80b0e90b4f60"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") });

            migrationBuilder.DeleteData(
                table: "PermissionRole",
                keyColumns: new[] { "PermissionsPermissionID", "RolesRoleID" },
                keyValues: new object[] { new Guid("9faeb1ff-0e3f-4faf-afa4-80b0e90b4f60"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") });

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "PermissionID",
                keyValue: new Guid("9faeb1ff-0e3f-4faf-afa4-80b0e90b4f60"));
        }
    }
}
