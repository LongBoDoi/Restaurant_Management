using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatbotConversation_Customer_CustomerID",
                table: "ChatbotConversation");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomMenuRequest_Customer_CustomerID",
                table: "CustomMenuRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_InventoryItemCategory_InventoryItemCategoryID",
                table: "InventoryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_MenuItem_MenuItemID",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Customer_CustomerID",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Table_Area_AreaID",
                table: "Table");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_Employee_EmployeeID",
                table: "UserLogin");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerID",
                table: "CustomMenuRequest",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatbotConversation_Customer_CustomerID",
                table: "ChatbotConversation",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomMenuRequest_Customer_CustomerID",
                table: "CustomMenuRequest",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_InventoryItemCategory_InventoryItemCategoryID",
                table: "InventoryItem",
                column: "InventoryItemCategoryID",
                principalTable: "InventoryItemCategory",
                principalColumn: "InventoryItemCategoryID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_MenuItem_MenuItemID",
                table: "OrderDetail",
                column: "MenuItemID",
                principalTable: "MenuItem",
                principalColumn: "MenuItemID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Customer_CustomerID",
                table: "Reservation",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Area_AreaID",
                table: "Table",
                column: "AreaID",
                principalTable: "Area",
                principalColumn: "AreaID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_Employee_EmployeeID",
                table: "UserLogin",
                column: "EmployeeID",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatbotConversation_Customer_CustomerID",
                table: "ChatbotConversation");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomMenuRequest_Customer_CustomerID",
                table: "CustomMenuRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_InventoryItemCategory_InventoryItemCategoryID",
                table: "InventoryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_MenuItem_MenuItemID",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Customer_CustomerID",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Table_Area_AreaID",
                table: "Table");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_Employee_EmployeeID",
                table: "UserLogin");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerID",
                table: "CustomMenuRequest",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatbotConversation_Customer_CustomerID",
                table: "ChatbotConversation",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomMenuRequest_Customer_CustomerID",
                table: "CustomMenuRequest",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_InventoryItemCategory_InventoryItemCategoryID",
                table: "InventoryItem",
                column: "InventoryItemCategoryID",
                principalTable: "InventoryItemCategory",
                principalColumn: "InventoryItemCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_MenuItem_MenuItemID",
                table: "OrderDetail",
                column: "MenuItemID",
                principalTable: "MenuItem",
                principalColumn: "MenuItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Customer_CustomerID",
                table: "Reservation",
                column: "CustomerID",
                principalTable: "Customer",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Table_Area_AreaID",
                table: "Table",
                column: "AreaID",
                principalTable: "Area",
                principalColumn: "AreaID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_Employee_EmployeeID",
                table: "UserLogin",
                column: "EmployeeID",
                principalTable: "Employee",
                principalColumn: "EmployeeID");
        }
    }
}
