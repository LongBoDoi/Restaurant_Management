using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleAndPermissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    PermissionID = table.Column<Guid>(type: "char(36)", nullable: false),
                    PermissionCode = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    PermissionName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.PermissionID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<Guid>(type: "char(36)", nullable: false),
                    RoleCode = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    RoleName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "EmployeeRole",
                columns: table => new
                {
                    EmployeesEmployeeID = table.Column<Guid>(type: "char(36)", nullable: false),
                    RolesRoleID = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRole", x => new { x.EmployeesEmployeeID, x.RolesRoleID });
                    table.ForeignKey(
                        name: "FK_EmployeeRole_Employee_EmployeesEmployeeID",
                        column: x => x.EmployeesEmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeRole_Role_RolesRoleID",
                        column: x => x.RolesRoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "PermissionRole",
                columns: table => new
                {
                    PermissionsPermissionID = table.Column<Guid>(type: "char(36)", nullable: false),
                    RolesRoleID = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRole", x => new { x.PermissionsPermissionID, x.RolesRoleID });
                    table.ForeignKey(
                        name: "FK_PermissionRole_Permission_PermissionsPermissionID",
                        column: x => x.PermissionsPermissionID,
                        principalTable: "Permission",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionRole_Role_RolesRoleID",
                        column: x => x.RolesRoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "EmployeeID",
                keyValue: new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"),
                columns: new[] { "Email", "PhoneNumber" },
                values: new object[] { "", "" });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "PermissionID", "CreatedDate", "ModifiedDate", "PermissionCode", "PermissionName" },
                values: new object[,]
                {
                    { new Guid("0b87ffbc-c7c7-482a-af25-ceb2c9f2daf2"), null, null, "ViewReport", "Xem báo cáo doanh thu" },
                    { new Guid("3978b007-d63c-442c-bcc1-600dcc251299"), null, null, "ManagePermission", "Quản lý phân quyền" },
                    { new Guid("3d1f0881-6469-4255-901f-81cef0298029"), null, null, "ManageMenu", "Quản lý thực đơn" },
                    { new Guid("45dbc11e-49ff-49a2-8f26-ca1823bf7a34"), null, null, "ManageOrder", "Quản lý order" },
                    { new Guid("50180af9-4ce3-436e-9fd5-57df78da2d74"), null, null, "ManageCustomer", "Quản lý khách hàng" },
                    { new Guid("5db0fbde-6a21-41c9-9676-27a3332760b3"), null, null, "ManageTable", "Quản lý bàn, đặt bàn" },
                    { new Guid("5dcb6d2a-d5bc-42f2-a184-7c317c30a67d"), null, null, "ManageEmployee", "Quản lý nhân viên" },
                    { new Guid("bf448b0b-b292-44b0-b563-69b78096fe84"), null, null, "ManageInventory", "Quản lý nguyên liệu" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleID", "CreatedDate", "ModifiedDate", "RoleCode", "RoleName" },
                values: new object[,]
                {
                    { new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), null, null, "AD", "Quản trị viên" },
                    { new Guid("7674405c-363b-48f8-8533-d20d301731b3"), null, null, "CSH", "Thu ngân" },
                    { new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), null, null, "MNG", "Quản lý" },
                    { new Guid("d5f40283-a427-4731-9548-c83e1c714da6"), null, null, "SRV", "Phục vụ" }
                });

            migrationBuilder.InsertData(
                table: "EmployeeRole",
                columns: new[] { "EmployeesEmployeeID", "RolesRoleID" },
                values: new object[] { new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") });

            migrationBuilder.InsertData(
                table: "PermissionRole",
                columns: new[] { "PermissionsPermissionID", "RolesRoleID" },
                values: new object[,]
                {
                    { new Guid("0b87ffbc-c7c7-482a-af25-ceb2c9f2daf2"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") },
                    { new Guid("0b87ffbc-c7c7-482a-af25-ceb2c9f2daf2"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") },
                    { new Guid("3978b007-d63c-442c-bcc1-600dcc251299"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") },
                    { new Guid("3978b007-d63c-442c-bcc1-600dcc251299"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") },
                    { new Guid("3d1f0881-6469-4255-901f-81cef0298029"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") },
                    { new Guid("3d1f0881-6469-4255-901f-81cef0298029"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") },
                    { new Guid("45dbc11e-49ff-49a2-8f26-ca1823bf7a34"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") },
                    { new Guid("45dbc11e-49ff-49a2-8f26-ca1823bf7a34"), new Guid("7674405c-363b-48f8-8533-d20d301731b3") },
                    { new Guid("45dbc11e-49ff-49a2-8f26-ca1823bf7a34"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") },
                    { new Guid("45dbc11e-49ff-49a2-8f26-ca1823bf7a34"), new Guid("d5f40283-a427-4731-9548-c83e1c714da6") },
                    { new Guid("50180af9-4ce3-436e-9fd5-57df78da2d74"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") },
                    { new Guid("50180af9-4ce3-436e-9fd5-57df78da2d74"), new Guid("7674405c-363b-48f8-8533-d20d301731b3") },
                    { new Guid("50180af9-4ce3-436e-9fd5-57df78da2d74"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") },
                    { new Guid("5db0fbde-6a21-41c9-9676-27a3332760b3"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") },
                    { new Guid("5db0fbde-6a21-41c9-9676-27a3332760b3"), new Guid("7674405c-363b-48f8-8533-d20d301731b3") },
                    { new Guid("5db0fbde-6a21-41c9-9676-27a3332760b3"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") },
                    { new Guid("5db0fbde-6a21-41c9-9676-27a3332760b3"), new Guid("d5f40283-a427-4731-9548-c83e1c714da6") },
                    { new Guid("5dcb6d2a-d5bc-42f2-a184-7c317c30a67d"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") },
                    { new Guid("5dcb6d2a-d5bc-42f2-a184-7c317c30a67d"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") },
                    { new Guid("bf448b0b-b292-44b0-b563-69b78096fe84"), new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") },
                    { new Guid("bf448b0b-b292-44b0-b563-69b78096fe84"), new Guid("b85db478-0a75-4561-91ab-acdc730e9990") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRole_RolesRoleID",
                table: "EmployeeRole",
                column: "RolesRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_PermissionCode",
                table: "Permission",
                column: "PermissionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRole_RolesRoleID",
                table: "PermissionRole",
                column: "RolesRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Role_RoleCode",
                table: "Role",
                column: "RoleCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeRole");

            migrationBuilder.DropTable(
                name: "PermissionRole");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "EmployeeID",
                keyValue: new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"),
                columns: new[] { "Email", "PhoneNumber" },
                values: new object[] { null, null });
        }
    }
}
