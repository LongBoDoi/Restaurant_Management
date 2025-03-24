using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuItemCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "MenuItem");

            migrationBuilder.AddColumn<Guid>(
                name: "MenuItemCategoryID",
                table: "MenuItem",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MenuItemCategory",
                columns: table => new
                {
                    MenuItemCategoryID = table.Column<Guid>(type: "char(36)", nullable: false),
                    MenuItemCategoryName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Inactive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemCategory", x => x.MenuItemCategoryID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.InsertData(
                table: "MenuItemCategory",
                columns: new[] { "MenuItemCategoryID", "CreatedDate", "Description", "Inactive", "MenuItemCategoryName", "ModifiedDate", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("758296ed-75e6-45c6-8a1e-b075524027af"), null, "", false, "Món chính", null, 1 },
                    { new Guid("78ef8d8c-a68e-40c9-99ce-5bb496faef2b"), null, "", false, "Đồ uống", null, 3 },
                    { new Guid("87de53a6-68e2-46a3-b998-7df936dfa1c5"), null, "", false, "Tráng miệng", null, 2 },
                    { new Guid("aa5e2d3f-4deb-4434-9ef2-19f5e51f21ad"), null, "", false, "Khai vị", null, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_MenuItemCategoryID",
                table: "MenuItem",
                column: "MenuItemCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_MenuItemCategory_MenuItemCategoryID",
                table: "MenuItem",
                column: "MenuItemCategoryID",
                principalTable: "MenuItemCategory",
                principalColumn: "MenuItemCategoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_MenuItemCategory_MenuItemCategoryID",
                table: "MenuItem");

            migrationBuilder.DropTable(
                name: "MenuItemCategory");

            migrationBuilder.DropIndex(
                name: "IX_MenuItem_MenuItemCategoryID",
                table: "MenuItem");

            migrationBuilder.DropColumn(
                name: "MenuItemCategoryID",
                table: "MenuItem");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "MenuItem",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
