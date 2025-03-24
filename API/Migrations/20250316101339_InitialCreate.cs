using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomerName = table.Column<string>(type: "longtext", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(255)", nullable: false),
                    Address = table.Column<string>(type: "longtext", nullable: false),
                    Preferences = table.Column<string>(type: "longtext", nullable: true),
                    LoyaltyPoint = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeID = table.Column<Guid>(type: "char(36)", nullable: false),
                    EmployeeCode = table.Column<string>(type: "varchar(255)", nullable: false),
                    EmployeeName = table.Column<string>(type: "longtext", nullable: false),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Schedule = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployeeID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "InventoryItem",
                columns: table => new
                {
                    InventoryItemID = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "longtext", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItem", x => x.InventoryItemID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "MenuItem",
                columns: table => new
                {
                    MenuItemID = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    OutOfStock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItem", x => x.MenuItemID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    SettingID = table.Column<Guid>(type: "char(36)", nullable: false),
                    SettingKey = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    SettingValue = table.Column<string>(type: "longtext", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.SettingID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ChatbotConversation",
                columns: table => new
                {
                    ConversationID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomerID = table.Column<Guid>(type: "char(36)", nullable: true),
                    Topic = table.Column<string>(type: "longtext", nullable: false),
                    IsHelpful = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatbotConversation", x => x.ConversationID);
                    table.ForeignKey(
                        name: "FK_ChatbotConversation_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomerID = table.Column<Guid>(type: "char(36)", nullable: true),
                    CustomerName = table.Column<string>(type: "longtext", nullable: false),
                    OrderName = table.Column<string>(type: "longtext", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SpecialRequest = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Order_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    ReservationID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomerID = table.Column<Guid>(type: "char(36)", nullable: true),
                    CustomerName = table.Column<string>(type: "longtext", nullable: false),
                    CustomerPhoneNumber = table.Column<string>(type: "longtext", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ReservationInfo = table.Column<string>(type: "longtext", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GuestCount = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.ReservationID);
                    table.ForeignKey(
                        name: "FK_Reservation_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    UserLoginID = table.Column<Guid>(type: "char(36)", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "char(36)", nullable: true),
                    CustomerID = table.Column<Guid>(type: "char(36)", nullable: true),
                    Username = table.Column<string>(type: "longtext", nullable: false),
                    Password = table.Column<string>(type: "longtext", nullable: false),
                    Token = table.Column<string>(type: "longtext", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => x.UserLoginID);
                    table.ForeignKey(
                        name: "FK_UserLogin_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                    table.ForeignKey(
                        name: "FK_UserLogin_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ChatbotConversationDetail",
                columns: table => new
                {
                    ConversationDetailID = table.Column<Guid>(type: "char(36)", nullable: false),
                    ConversationID = table.Column<Guid>(type: "char(36)", nullable: false),
                    Sender = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatbotConversationDetail", x => x.ConversationDetailID);
                    table.ForeignKey(
                        name: "FK_ChatbotConversationDetail_ChatbotConversation_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "ChatbotConversation",
                        principalColumn: "ConversationID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailID = table.Column<Guid>(type: "char(36)", nullable: false),
                    OrderID = table.Column<Guid>(type: "char(36)", nullable: false),
                    MenuItemID = table.Column<Guid>(type: "char(36)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_OrderDetail_MenuItem_MenuItemID",
                        column: x => x.MenuItemID,
                        principalTable: "MenuItem",
                        principalColumn: "MenuItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "EmployeeID", "CreatedDate", "EmployeeCode", "EmployeeName", "ModifiedDate", "PhoneNumber", "Role", "Schedule" },
                values: new object[] { new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"), null, "admin", "Admin", null, null, 0, null });

            migrationBuilder.InsertData(
                table: "Setting",
                columns: new[] { "SettingID", "CreatedDate", "DataType", "ModifiedDate", "SettingKey", "SettingValue" },
                values: new object[,]
                {
                    { new Guid("06005218-029d-4040-9332-62e2e5dcb597"), null, 1, null, "IntroImageUrl", "" },
                    { new Guid("0b10c89c-7b7c-4f98-8828-1d07dffd2f73"), null, 1, null, "RestaurantName", "" },
                    { new Guid("3f07e912-5330-405d-9679-1d17f9b2eff4"), null, 1, null, "RestaurantAddress", "" },
                    { new Guid("401ef8c2-370c-470e-ad71-9924a85d18ff"), null, 1, null, "RestaurantSlogan", "" },
                    { new Guid("43e2553a-5a18-4258-8c14-1852564fb309"), null, 3, null, "DisplayMenuScreenForCustomer", "true" },
                    { new Guid("5ac03c72-3e46-4f70-9b5f-3160c9ee2327"), null, 2, null, "DisplayMenuScreenForCustomerType", "0" },
                    { new Guid("b9ea5327-bbda-4f44-ba55-ccf02ed1b7ff"), null, 2, null, "DisplayMenuScreenByItemsForCustomerType", "0" },
                    { new Guid("d69a097b-f337-4521-a302-d9ed9a876a5e"), null, 1, null, "ListMenuScreenForCustomerItems", "[]" },
                    { new Guid("f10e0c45-17d3-4715-802e-30a5b5abc14c"), null, 3, null, "DisplayBookingScreenForCustomer", "true" }
                });

            migrationBuilder.InsertData(
                table: "UserLogin",
                columns: new[] { "UserLoginID", "CreatedDate", "CustomerID", "EmployeeID", "ModifiedDate", "Password", "Token", "Username" },
                values: new object[] { new Guid("8b59dd9f-72d8-4d01-a971-03bc98c2262f"), null, null, new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"), null, "123456", "", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatbotConversation_CustomerID",
                table: "ChatbotConversation",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatbotConversationDetail_ConversationID",
                table: "ChatbotConversationDetail",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_PhoneNumber",
                table: "Customer",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeCode",
                table: "Employee",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerID",
                table: "Order",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_MenuItemID",
                table: "OrderDetail",
                column: "MenuItemID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderID",
                table: "OrderDetail",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_CustomerID",
                table: "Reservation",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Setting_SettingKey",
                table: "Setting",
                column: "SettingKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_CustomerID",
                table: "UserLogin",
                column: "CustomerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_EmployeeID",
                table: "UserLogin",
                column: "EmployeeID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatbotConversationDetail");

            migrationBuilder.DropTable(
                name: "InventoryItem");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "ChatbotConversation");

            migrationBuilder.DropTable(
                name: "MenuItem");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
