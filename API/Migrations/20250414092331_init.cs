using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    AreaID = table.Column<Guid>(type: "char(36)", nullable: false),
                    AreaName = table.Column<string>(type: "longtext", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomerName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
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
                    EmployeeCode = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    EmployeeName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    WorkStatus = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
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
                name: "InventoryItemCategory",
                columns: table => new
                {
                    InventoryItemCategoryID = table.Column<Guid>(type: "char(36)", nullable: false),
                    InventoryItemCategoryName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Inactive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemCategory", x => x.InventoryItemCategoryID);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "MenuItemCategory",
                columns: table => new
                {
                    MenuItemCategoryID = table.Column<Guid>(type: "char(36)", nullable: false),
                    MenuItemCategoryName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Inactive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemCategory", x => x.MenuItemCategoryID);
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
                name: "Table",
                columns: table => new
                {
                    TableID = table.Column<Guid>(type: "char(36)", nullable: false),
                    TableName = table.Column<string>(type: "longtext", nullable: false),
                    SeatCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AreaID = table.Column<Guid>(type: "char(36)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => x.TableID);
                    table.ForeignKey(
                        name: "FK_Table_Area_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Area",
                        principalColumn: "AreaID");
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
                name: "CustomMenuRequest",
                columns: table => new
                {
                    CustomMenuRequestID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomerID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomerName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    MenuItemName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomMenuRequest", x => x.CustomMenuRequestID);
                    table.ForeignKey(
                        name: "FK_CustomMenuRequest_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomerID = table.Column<Guid>(type: "char(36)", nullable: true),
                    CustomerName = table.Column<string>(type: "longtext", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    SpecialRequest = table.Column<string>(type: "longtext", nullable: true),
                    TableName = table.Column<string>(type: "longtext", nullable: true),
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
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLogin_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "InventoryItem",
                columns: table => new
                {
                    InventoryItemID = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WarningStockQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "longtext", nullable: false),
                    InventoryItemCategoryID = table.Column<Guid>(type: "char(36)", nullable: true),
                    Inactive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItem", x => x.InventoryItemID);
                    table.ForeignKey(
                        name: "FK_InventoryItem_InventoryItemCategory_InventoryItemCategoryID",
                        column: x => x.InventoryItemCategoryID,
                        principalTable: "InventoryItemCategory",
                        principalColumn: "InventoryItemCategoryID");
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "MenuItem",
                columns: table => new
                {
                    MenuItemID = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MenuItemCategoryID = table.Column<Guid>(type: "char(36)", nullable: true),
                    Inactive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItem", x => x.MenuItemID);
                    table.ForeignKey(
                        name: "FK_MenuItem_MenuItemCategory_MenuItemCategoryID",
                        column: x => x.MenuItemCategoryID,
                        principalTable: "MenuItemCategory",
                        principalColumn: "MenuItemCategoryID",
                        onDelete: ReferentialAction.SetNull);
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
                name: "OrderTable",
                columns: table => new
                {
                    OrderTableID = table.Column<Guid>(type: "char(36)", nullable: false),
                    OrderID = table.Column<Guid>(type: "char(36)", nullable: false),
                    TableID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTable", x => x.OrderTableID);
                    table.ForeignKey(
                        name: "FK_OrderTable_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTable_Table_TableID",
                        column: x => x.TableID,
                        principalTable: "Table",
                        principalColumn: "TableID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ReservationTable",
                columns: table => new
                {
                    ReservationTableID = table.Column<Guid>(type: "char(36)", nullable: false),
                    ReservationID = table.Column<Guid>(type: "char(36)", nullable: false),
                    TableID = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTable", x => x.ReservationTableID);
                    table.ForeignKey(
                        name: "FK_ReservationTable_Reservation_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "Reservation",
                        principalColumn: "ReservationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationTable_Table_TableID",
                        column: x => x.TableID,
                        principalTable: "Table",
                        principalColumn: "TableID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "CustomMenuRequestInventoryItem",
                columns: table => new
                {
                    CustomMenuRequestsCustomMenuRequestID = table.Column<Guid>(type: "char(36)", nullable: false),
                    InventoryItemsInventoryItemID = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomMenuRequestInventoryItem", x => new { x.CustomMenuRequestsCustomMenuRequestID, x.InventoryItemsInventoryItemID });
                    table.ForeignKey(
                        name: "FK_CustomMenuRequestInventoryItem_CustomMenuRequest_CustomMenuR~",
                        column: x => x.CustomMenuRequestsCustomMenuRequestID,
                        principalTable: "CustomMenuRequest",
                        principalColumn: "CustomMenuRequestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomMenuRequestInventoryItem_InventoryItem_InventoryItemsI~",
                        column: x => x.InventoryItemsInventoryItemID,
                        principalTable: "InventoryItem",
                        principalColumn: "InventoryItemID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "MenuItemInventoryItem",
                columns: table => new
                {
                    MenuItemID = table.Column<Guid>(type: "char(36)", nullable: false),
                    InventoryItemID = table.Column<Guid>(type: "char(36)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemInventoryItem", x => new { x.MenuItemID, x.InventoryItemID });
                    table.ForeignKey(
                        name: "FK_MenuItemInventoryItem_InventoryItem_InventoryItemID",
                        column: x => x.InventoryItemID,
                        principalTable: "InventoryItem",
                        principalColumn: "InventoryItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuItemInventoryItem_MenuItem_MenuItemID",
                        column: x => x.MenuItemID,
                        principalTable: "MenuItem",
                        principalColumn: "MenuItemID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailID = table.Column<Guid>(type: "char(36)", nullable: false),
                    OrderID = table.Column<Guid>(type: "char(36)", nullable: false),
                    MenuItemID = table.Column<Guid>(type: "char(36)", nullable: true),
                    MenuItemName = table.Column<string>(type: "longtext", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                        principalColumn: "MenuItemID");
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
                columns: new[] { "EmployeeID", "CreatedDate", "Email", "EmployeeCode", "EmployeeName", "ImageUrl", "ModifiedDate", "PhoneNumber", "Schedule", "WorkStatus" },
                values: new object[] { new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"), null, null, "admin", "Admin", null, null, null, null, 0 });

            migrationBuilder.InsertData(
                table: "MenuItemCategory",
                columns: new[] { "MenuItemCategoryID", "Color", "CreatedDate", "Description", "Inactive", "MenuItemCategoryName", "ModifiedDate", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("758296ed-75e6-45c6-8a1e-b075524027af"), "#F59E0B", null, "", false, "Món chính", null, 1 },
                    { new Guid("78ef8d8c-a68e-40c9-99ce-5bb496faef2b"), "#A855F7", null, "", false, "Đồ uống", null, 3 },
                    { new Guid("87de53a6-68e2-46a3-b998-7df936dfa1c5"), "#3B82F6", null, "", false, "Tráng miệng", null, 2 },
                    { new Guid("aa5e2d3f-4deb-4434-9ef2-19f5e51f21ad"), "#22C55E", null, "", false, "Khai vị", null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Setting",
                columns: new[] { "SettingID", "CreatedDate", "DataType", "ModifiedDate", "SettingKey", "SettingValue" },
                values: new object[,]
                {
                    { new Guid("06005218-029d-4040-9332-62e2e5dcb597"), null, 1, null, "IntroImageUrl", "" },
                    { new Guid("0b10c89c-7b7c-4f98-8828-1d07dffd2f73"), null, 1, null, "RestaurantName", "" },
                    { new Guid("3ca30af9-1538-4339-91d5-cd90a8ef44e4"), null, 1, null, "SocialMediaUrls", "[]" },
                    { new Guid("3f07e912-5330-405d-9679-1d17f9b2eff4"), null, 1, null, "RestaurantAddress", "" },
                    { new Guid("401ef8c2-370c-470e-ad71-9924a85d18ff"), null, 1, null, "RestaurantSlogan", "" },
                    { new Guid("43e2553a-5a18-4258-8c14-1852564fb309"), null, 3, null, "DisplayMenuScreenForCustomer", "true" },
                    { new Guid("4e5050c5-1a3c-482f-8169-e902fcca464f"), null, 1, null, "RestaurantPhoneNumber", "" },
                    { new Guid("5ac03c72-3e46-4f70-9b5f-3160c9ee2327"), null, 2, null, "DisplayMenuScreenForCustomerType", "0" },
                    { new Guid("69a4356b-1f0a-40d5-ab4d-a75a2f0e16fb"), null, 1, null, "OpeningTimes", "[]" },
                    { new Guid("6c9b5da9-56a8-40ad-8fa9-866e5f4cbd50"), null, 1, null, "MenuCategoryColors", "[\"#EF4444\",\"#3B82F6\",\"#22C55E\",\"#F59E0B\",\"#A855F7\",\"#EC4899\",\"#6366F1\",\"#14B8A6\"]" },
                    { new Guid("b9ea5327-bbda-4f44-ba55-ccf02ed1b7ff"), null, 2, null, "DisplayMenuScreenByItemsForCustomerType", "0" },
                    { new Guid("c5bfe361-32f5-4b12-ba50-fc877e88c1f9"), null, 1, null, "ListMenuScreenForCustomerImages", "[]" },
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
                name: "IX_CustomMenuRequest_CustomerID",
                table: "CustomMenuRequest",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomMenuRequestInventoryItem_InventoryItemsInventoryItemID",
                table: "CustomMenuRequestInventoryItem",
                column: "InventoryItemsInventoryItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeCode",
                table: "Employee",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_InventoryItemCategoryID",
                table: "InventoryItem",
                column: "InventoryItemCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_MenuItemCategoryID",
                table: "MenuItem",
                column: "MenuItemCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemInventoryItem_InventoryItemID",
                table: "MenuItemInventoryItem",
                column: "InventoryItemID");

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
                name: "IX_OrderTable_OrderID",
                table: "OrderTable",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTable_TableID",
                table: "OrderTable",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_CustomerID",
                table: "Reservation",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTable_ReservationID",
                table: "ReservationTable",
                column: "ReservationID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTable_TableID",
                table: "ReservationTable",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_Setting_SettingKey",
                table: "Setting",
                column: "SettingKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Table_AreaID",
                table: "Table",
                column: "AreaID");

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
                name: "CustomMenuRequestInventoryItem");

            migrationBuilder.DropTable(
                name: "MenuItemInventoryItem");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "OrderTable");

            migrationBuilder.DropTable(
                name: "ReservationTable");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "ChatbotConversation");

            migrationBuilder.DropTable(
                name: "CustomMenuRequest");

            migrationBuilder.DropTable(
                name: "InventoryItem");

            migrationBuilder.DropTable(
                name: "MenuItem");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "Table");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "InventoryItemCategory");

            migrationBuilder.DropTable(
                name: "MenuItemCategory");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Area");
        }
    }
}
