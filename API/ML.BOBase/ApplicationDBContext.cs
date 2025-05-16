using API.ML.BO;
using API.ML.Common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;

namespace API.ML.BOBase
{
    public class ApplicationDBContext : DbContext
    {
        #region Tables
        /// <summary>
        /// Bảng bài test
        /// </summary>
        public DbSet<Employee> Employee { get; set; }

        /// <summary>
        /// Bảng bài tập
        /// </summary>
        public DbSet<Customer> Customer { get; set; }

        /// <summary>
        /// Bảng câu hỏi
        /// </summary>
        public DbSet<InventoryItem> InventoryItem { get; set; }

        /// <summary>
        /// Bảng câu trả lời
        /// </summary>
        public DbSet<MenuItem> MenuItem { get; set; }

        /// <summary>
        /// Bảng lần làm bài
        /// </summary>
        public DbSet<BO.Order> Order { get; set; }

        /// <summary>
        /// Bảng chi tiết lần làm bài
        /// </summary>
        public DbSet<OrderDetail> OrderDetail { get; set; }

        /// <summary>
        /// Bảng người dùng
        /// </summary>
        public DbSet<UserLogin> UserLogin { get; set; }

        /// <summary>
        /// Bảng đặt bàn
        /// </summary>
        public DbSet<Reservation> Reservation { get; set; }

        /// <summary>
        /// Bảng hội thoại
        /// </summary>
        public DbSet<ChatbotConversation> ChatbotConversation { get; set; }

        /// <summary>
        /// bảng tin nhắn hội thoại
        /// </summary>
        public DbSet<ChatbotConversationDetail> ChatbotConversationDetail { get; set; }

        /// <summary>
        /// Bảng thiết lập
        /// </summary>
        public DbSet<Setting> Setting { get; set; }

        /// <summary>
        /// Bảng nhóm thực đơn
        /// </summary>
        public DbSet<MenuItemCategory> MenuItemCategory { get; set; }

        /// <summary>
        /// Bảng bàn
        /// </summary>
        public DbSet<Table> Table { get; set; }

        /// <summary>
        /// Bảng khu vực
        /// </summary>
        public DbSet<Area> Area { get; set; }

        /// <summary>
        /// Bảng nhóm nguyên liệu
        /// </summary>
        public DbSet<InventoryItemCategory> InventoryItemCategory { get; set; }

        /// <summary>
        /// Bảng liên kết Đặt chỗ và Bàn
        /// </summary>
        public DbSet<ReservationTable> ReservationTable { get; set; }

        /// <summary>
        /// Bảng liên kết Order và bàn
        /// </summary>
        public DbSet<OrderTable> OrderTable { get; set; }

        /// <summary>
        /// Bảng yêu cầu tạo món custom
        /// </summary>
        public DbSet<CustomMenuRequest> CustomMenuRequest { get; set; }

        /// <summary>
        /// Bảng vai trò
        /// </summary>
        public DbSet<Role> Role { get; set; }

        /// <summary>
        /// Bảng phân quyền
        /// </summary>
        public DbSet<Permission> Permission { get; set; }
        #endregion

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
        {
        }

        /// <summary>
        /// Lưu thay đổi
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            PrepareDataBeforeSave();

            using (var transaction = Database.BeginTransaction())
            {
                try
                {
                    var result = base.SaveChanges();
                    transaction.Commit();
                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Lưu thay đổi Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            PrepareDataBeforeSave();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Khởi tạo model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.Customer).WithMany(o => o.Orders).HasForeignKey(od => od.CustomerID);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasOne(od => od.Order).WithMany(o => o.OrderDetails).HasForeignKey(od => od.OrderID);

                entity.HasOne(od => od.MenuItem).WithMany(mi => mi.OrderDetails).HasForeignKey(od => od.MenuItemID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ChatbotConversation>(entity =>
            {
                entity.HasOne(cc => cc.Customer).WithMany(c => c.ChatbotConversations).HasForeignKey(cc => cc.CustomerID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatbotConversationDetail>(entity =>
            {
                entity.HasOne(ccd => ccd.ChatbotConversation).WithMany(cc => cc.ChatbotConversationDetails).HasForeignKey(ccd => ccd.ConversationID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Reservation>(r =>
            {
                r.HasOne(r => r.Customer).WithMany(c => c.Reservations).HasForeignKey(r => r.CustomerID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Customer>(c =>
            {
                c.HasIndex(c => c.PhoneNumber).IsUnique();
            });

            modelBuilder.Entity<Employee>(e =>
            {
                e.HasIndex(c => c.EmployeeCode).IsUnique();
                e.HasData(
                    new Employee { EmployeeID = new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"), EmployeeCode = "admin", EmployeeName = "Admin", WorkStatus = EnumEmployeeWorkStatus.Active }
                );

                // Cấp quyền admin cho tài khoản admin
                e.HasMany(e => e.Roles).WithMany(r => r.Employees).UsingEntity(j => j.HasData(
                    new { EmployeesEmployeeID = new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"), RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3") }
                ));
            });

            var manageMenuPermission = new Permission { PermissionID = new Guid("3d1f0881-6469-4255-901f-81cef0298029"), PermissionCode = "ManageMenu", PermissionName = "Quản lý thực đơn" };
            var manageInventoryPermission = new Permission { PermissionID = new Guid("bf448b0b-b292-44b0-b563-69b78096fe84"), PermissionCode = "ManageInventory", PermissionName = "Quản lý nguyên liệu" };
            var manageTablePermission = new Permission { PermissionID = new Guid("5db0fbde-6a21-41c9-9676-27a3332760b3"), PermissionCode = "ManageTable", PermissionName = "Quản lý bàn, đặt bàn" };
            var manageOrderPermission = new Permission { PermissionID = new Guid("45dbc11e-49ff-49a2-8f26-ca1823bf7a34"), PermissionCode = "ManageOrder", PermissionName = "Quản lý order" };
            var manageCustomerPermission = new Permission { PermissionID = new Guid("50180af9-4ce3-436e-9fd5-57df78da2d74"), PermissionCode = "ManageCustomer", PermissionName = "Quản lý khách hàng" };
            var manageEmployeePermission = new Permission { PermissionID = new Guid("5dcb6d2a-d5bc-42f2-a184-7c317c30a67d"), PermissionCode = "ManageEmployee", PermissionName = "Quản lý nhân viên" };
            var manageRolePermission = new Permission { PermissionID = new Guid("3978b007-d63c-442c-bcc1-600dcc251299"), PermissionCode = "ManagePermission", PermissionName = "Quản lý phân quyền" };
            var viewReportPermission = new Permission { PermissionID = new Guid("0b87ffbc-c7c7-482a-af25-ceb2c9f2daf2"), PermissionCode = "ViewReport", PermissionName = "Xem báo cáo doanh thu" };
            var manageSettingPermission = new Permission { PermissionID = new Guid("9faeb1ff-0e3f-4faf-afa4-80b0e90b4f60"), PermissionCode = "ManageSetting", PermissionName = "Quản lý thiết lập" };

            modelBuilder.Entity<Role>(r =>
            {
                r.HasIndex(r => r.RoleCode).IsUnique();
                r.HasData(
                    new Role { RoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), RoleCode = "AD", RoleName = "Quản trị viên" },
                    new Role { RoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), RoleCode = "MNG", RoleName = "Quản lý" },
                    new Role { RoleID = new Guid("7674405c-363b-48f8-8533-d20d301731b3"), RoleCode = "CSH", RoleName = "Thu ngân" },
                    new Role { RoleID = new Guid("d5f40283-a427-4731-9548-c83e1c714da6"), RoleCode = "SRV", RoleName = "Phục vụ" }
                );

                // Cấp quyền mặc định cho các vai trò
                r.HasMany(r => r.Permissions).WithMany(p => p.Roles).UsingEntity(j => j.HasData(
                    // Quản trị viên
                    new { RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), PermissionsPermissionID = manageMenuPermission.PermissionID },
                    new { RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), PermissionsPermissionID = manageInventoryPermission.PermissionID },
                    new { RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), PermissionsPermissionID = manageTablePermission.PermissionID },
                    new { RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), PermissionsPermissionID = manageOrderPermission.PermissionID },
                    new { RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), PermissionsPermissionID = manageCustomerPermission.PermissionID },
                    new { RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), PermissionsPermissionID = manageEmployeePermission.PermissionID },
                    new { RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), PermissionsPermissionID = manageRolePermission.PermissionID },
                    new { RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), PermissionsPermissionID = viewReportPermission.PermissionID },
                    new { RolesRoleID = new Guid("4a2ef244-b552-498b-918b-cc18fd2afbf3"), PermissionsPermissionID = manageSettingPermission.PermissionID },

                    // Quản lý
                    new { RolesRoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), PermissionsPermissionID = manageMenuPermission.PermissionID },
                    new { RolesRoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), PermissionsPermissionID = manageInventoryPermission.PermissionID },
                    new { RolesRoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), PermissionsPermissionID = manageTablePermission.PermissionID },
                    new { RolesRoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), PermissionsPermissionID = manageOrderPermission.PermissionID },
                    new { RolesRoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), PermissionsPermissionID = manageCustomerPermission.PermissionID },
                    new { RolesRoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), PermissionsPermissionID = manageEmployeePermission.PermissionID },
                    new { RolesRoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), PermissionsPermissionID = manageRolePermission.PermissionID },
                    new { RolesRoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), PermissionsPermissionID = viewReportPermission.PermissionID },
                    new { RolesRoleID = new Guid("b85db478-0a75-4561-91ab-acdc730e9990"), PermissionsPermissionID = manageSettingPermission.PermissionID },

                    // Thu ngân
                    new { RolesRoleID = new Guid("7674405c-363b-48f8-8533-d20d301731b3"), PermissionsPermissionID = manageTablePermission.PermissionID },
                    new { RolesRoleID = new Guid("7674405c-363b-48f8-8533-d20d301731b3"), PermissionsPermissionID = manageOrderPermission.PermissionID },
                    new { RolesRoleID = new Guid("7674405c-363b-48f8-8533-d20d301731b3"), PermissionsPermissionID = manageCustomerPermission.PermissionID },

                    // Phục vụ
                    new { RolesRoleID = new Guid("d5f40283-a427-4731-9548-c83e1c714da6"), PermissionsPermissionID = manageTablePermission.PermissionID },
                    new { RolesRoleID = new Guid("d5f40283-a427-4731-9548-c83e1c714da6"), PermissionsPermissionID = manageOrderPermission.PermissionID }
                ));
            });

            modelBuilder.Entity<Permission>(r =>
            {
                r.HasIndex(r => r.PermissionCode).IsUnique();
                r.HasData(
                    manageMenuPermission,
                    manageInventoryPermission,
                    manageTablePermission,
                    manageOrderPermission,
                    manageCustomerPermission,
                    manageEmployeePermission,
                    manageRolePermission,
                    viewReportPermission,
                    manageSettingPermission
                );
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasOne(ul => ul.Employee).WithOne(e => e.UserLogin).HasForeignKey<UserLogin>(ul => ul.EmployeeID)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ul => ul.Customer).WithOne(c => c.UserLogin).HasForeignKey<UserLogin>(ul => ul.CustomerID)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasData(
                    new UserLogin { UserLoginID = new Guid("8b59dd9f-72d8-4d01-a971-03bc98c2262f"), EmployeeID = new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"), Username = "admin", Password = "AQAAAAIAAYagAAAAEKqe/dcgGzIC8jgfJymczuCBpLYck9TdfxOQ19M6h9o4qBbTxjCk8PP2fzb49fPGPQ==" }
                );
            });

            modelBuilder.Entity<Setting>(s =>
            {
                s.HasIndex(s => s.SettingKey).IsUnique();
                s.HasData(
                    new Setting { SettingID = new Guid("06005218-029d-4040-9332-62e2e5dcb597"), SettingKey = "IntroImageUrl" },
                    new Setting { SettingID = new Guid("0b10c89c-7b7c-4f98-8828-1d07dffd2f73"), SettingKey = "RestaurantName" },
                    new Setting { SettingID = new Guid("3f07e912-5330-405d-9679-1d17f9b2eff4"), SettingKey = "RestaurantAddress" },
                    new Setting { SettingID = new Guid("4e5050c5-1a3c-482f-8169-e902fcca464f"), SettingKey = "RestaurantPhoneNumber" },
                    new Setting { SettingID = new Guid("3ca30af9-1538-4339-91d5-cd90a8ef44e4"), SettingKey = "SocialMediaUrls", SettingValue = "[]" },
                    new Setting { SettingID = new Guid("69a4356b-1f0a-40d5-ab4d-a75a2f0e16fb"), SettingKey = "OpeningTimes", SettingValue = "[]" },
                    new Setting { SettingID = new Guid("401ef8c2-370c-470e-ad71-9924a85d18ff"), SettingKey = "RestaurantSlogan" },
                    new Setting { SettingID = new Guid("43e2553a-5a18-4258-8c14-1852564fb309"), SettingKey = "DisplayMenuScreenForCustomer", SettingValue = "true", DataType = EnumDataType.Boolean },
                    new Setting { SettingID = new Guid("5ac03c72-3e46-4f70-9b5f-3160c9ee2327"), SettingKey = "DisplayMenuScreenForCustomerType", SettingValue = $"{(int)EnumDisplayMenuScreenForCustomerType.ByImages}", DataType = EnumDataType.Integer },
                    new Setting { SettingID = new Guid("b9ea5327-bbda-4f44-ba55-ccf02ed1b7ff"), SettingKey = "DisplayMenuScreenByItemsForCustomerType", SettingValue = $"{(int)EnumDisplayMenuScreenByItemsForCustomerType.All}", DataType = EnumDataType.Integer },
                    new Setting { SettingID = new Guid("c5bfe361-32f5-4b12-ba50-fc877e88c1f9"), SettingKey = "ListMenuScreenForCustomerImages", SettingValue = "[]" },
                    new Setting { SettingID = new Guid("d69a097b-f337-4521-a302-d9ed9a876a5e"), SettingKey = "ListMenuScreenForCustomerItems", SettingValue = "[]" },
                    new Setting { SettingID = new Guid("f10e0c45-17d3-4715-802e-30a5b5abc14c"), SettingKey = "DisplayBookingScreenForCustomer", SettingValue = "true", DataType = EnumDataType.Boolean },
                    new Setting { SettingID = new Guid("6c9b5da9-56a8-40ad-8fa9-866e5f4cbd50"), SettingKey = "MenuCategoryColors", SettingValue = $"{JsonConvert.SerializeObject(new string[] { "#EF4444", "#3B82F6", "#22C55E", "#F59E0B", "#A855F7", "#EC4899", "#6366F1", "#14B8A6" })}" }
                );
            });

            modelBuilder.Entity<MenuItem>(mi =>
            {
                mi.HasOne(mi => mi.MenuItemCategory).WithMany(mic => mic.MenuItems).HasForeignKey(mi => mi.MenuItemCategoryID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<MenuItemCategory>(mic =>
            {
                mic.HasData(
                    new MenuItemCategory { MenuItemCategoryID = new Guid("aa5e2d3f-4deb-4434-9ef2-19f5e51f21ad"), MenuItemCategoryName = "Khai vị", Color = "#22C55E", SortOrder = 0 },
                    new MenuItemCategory { MenuItemCategoryID = new Guid("758296ed-75e6-45c6-8a1e-b075524027af"), MenuItemCategoryName = "Món chính", Color = "#F59E0B", SortOrder = 1 },
                    new MenuItemCategory { MenuItemCategoryID = new Guid("87de53a6-68e2-46a3-b998-7df936dfa1c5"), MenuItemCategoryName = "Tráng miệng", Color = "#3B82F6", SortOrder = 2 },
                    new MenuItemCategory { MenuItemCategoryID = new Guid("78ef8d8c-a68e-40c9-99ce-5bb496faef2b"), MenuItemCategoryName = "Đồ uống", Color = "#A855F7", SortOrder = 3 }
                );
            });

            modelBuilder.Entity<InventoryItem>(mi =>
            {
                mi.HasOne(mi => mi.InventoryItemCategory).WithMany(mic => mic.InventoryItems).HasForeignKey(mi => mi.InventoryItemCategoryID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<MenuItemInventoryItem>(miii =>
            {
                miii.HasKey(sc => new { sc.MenuItemID, sc.InventoryItemID });

                miii.HasOne(sc => sc.MenuItem)
                    .WithMany(s => s.MenuItemInventoryItems)
                    .HasForeignKey(sc => sc.MenuItemID)
                    .OnDelete(DeleteBehavior.Cascade);

                miii.HasOne(e => e.InventoryItem)
                    .WithMany(e => e.MenuItemInventoryItems)
                    .HasForeignKey(e => e.InventoryItemID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ReservationTable>(rt =>
            {
                rt.HasOne(rt => rt.Reservation)
                    .WithMany(r => r.ReservationTables)
                    .HasForeignKey(rt => rt.ReservationID)
                    .OnDelete(DeleteBehavior.Cascade);

                rt.HasOne(e => e.Table)
                    .WithMany(e => e.ReservationTables)
                    .HasForeignKey(e => e.TableID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderTable>(ot =>
            {
                ot.HasOne(ot => ot.Order)
                    .WithMany(o => o.OrderTables)
                    .HasForeignKey(ot => ot.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

                ot.HasOne(ot => ot.Table)
                    .WithMany(t => t.OrderTables)
                    .HasForeignKey(e => e.TableID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Table>(t =>
            {
                t.HasOne(t => t.Area).WithMany(a => a.Tables).HasForeignKey(t => t.AreaID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<CustomMenuRequest>(entity =>
            {
                entity.HasOne(cmr => cmr.Customer).WithMany(c => c.CustomMenuRequests).HasForeignKey(cmr => cmr.CustomerID)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(cmr => cmr.InventoryItems).WithMany(ii => ii.CustomMenuRequests);
            });
        }

        /// <summary>
        /// Cập nhật CreatedDate và ModifiedDate
        /// </summary>
        private void PrepareDataBeforeSave()
        {
            var entities = ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entities)
            {
                if (entityEntry.Entity is IMLEntity entity)
                {
                    // Update timestamps
                    entity.ModifiedDate = DateTime.UtcNow;

                    if (entityEntry.State == EntityState.Added)
                    {
                        entity.CreatedDate = entity.ModifiedDate;
                    }

                    // Trimming
                    var stringProperties = entity.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                    foreach (var prop in stringProperties)
                    {
                        var currentValue = prop.GetValue(entity) as string;
                        if (!string.IsNullOrEmpty(currentValue))
                        {
                            prop.SetValue(entity, currentValue.Trim());
                        }
                    }
                }
            }
        }
    }
}
