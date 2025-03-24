using API.ML.BO;
using API.ML.Common;
using Microsoft.EntityFrameworkCore;

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
            UpdateTimestamps();
            return base.SaveChanges();
        }

        /// <summary>
        /// Lưu thay đổi Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
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

                entity.HasOne(od => od.MenuItem).WithMany(mi => mi.OrderDetails).HasForeignKey(od => od.MenuItemID);
            });

            modelBuilder.Entity<ChatbotConversation>(entity =>
            {
                entity.HasOne(cc => cc.Customer).WithMany(c => c.ChatbotConversations).HasForeignKey(cc => cc.CustomerID);
            });

            modelBuilder.Entity<ChatbotConversationDetail>(entity =>
            {
                entity.HasOne(ccd => ccd.ChatbotConversation).WithMany(cc => cc.ChatbotConversationDetails).HasForeignKey(ccd => ccd.ConversationID);
            });

            modelBuilder.Entity<Reservation>(r =>
            {
                r.HasOne(r => r.Customer).WithMany(c => c.Reservations).HasForeignKey(r => r.CustomerID);
            });

            modelBuilder.Entity<Customer>(c =>
            {
                c.HasIndex(c => c.PhoneNumber).IsUnique();
            });

            modelBuilder.Entity<Employee>(e =>
            {
                e.HasIndex(c => c.EmployeeCode).IsUnique();
                e.HasData(
                    new { EmployeeID = new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"), EmployeeCode = "admin", EmployeeName = "Admin", Role = EnumRole.Admin }
                );
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasOne(ul => ul.Employee).WithOne(e => e.UserLogin).HasForeignKey<UserLogin>(ul => ul.EmployeeID);
                entity.HasOne(ul => ul.Customer).WithOne(c => c.UserLogin).HasForeignKey<UserLogin>(ul => ul.CustomerID);
                entity.HasData(
                    new UserLogin { UserLoginID = new Guid("8b59dd9f-72d8-4d01-a971-03bc98c2262f"), EmployeeID = new Guid("d0929aef-1a5b-44f6-962d-01f7f9bb2b2b"), Username = "admin", Password = "123456" }
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
                    new Setting { SettingID = new Guid("f10e0c45-17d3-4715-802e-30a5b5abc14c"), SettingKey = "DisplayBookingScreenForCustomer", SettingValue = "true", DataType = EnumDataType.Boolean }
                );
            });

            modelBuilder.Entity<MenuItem>(mi =>
            {
                mi.HasOne(mi => mi.MenuItemCategory).WithMany(mic => mic.MenuItems).HasForeignKey(mi => mi.MenuItemCategoryID);
            });

            modelBuilder.Entity<MenuItemCategory>(mic =>
            {
                mic.HasData(
                    new MenuItemCategory { MenuItemCategoryID = new Guid("aa5e2d3f-4deb-4434-9ef2-19f5e51f21ad"), MenuItemCategoryName = "Khai vị", SortOrder = 0 },
                    new MenuItemCategory { MenuItemCategoryID = new Guid("758296ed-75e6-45c6-8a1e-b075524027af"), MenuItemCategoryName = "Món chính", SortOrder = 1 },
                    new MenuItemCategory { MenuItemCategoryID = new Guid("87de53a6-68e2-46a3-b998-7df936dfa1c5"), MenuItemCategoryName = "Tráng miệng", SortOrder = 2 },
                    new MenuItemCategory { MenuItemCategoryID = new Guid("78ef8d8c-a68e-40c9-99ce-5bb496faef2b"), MenuItemCategoryName = "Đồ uống", SortOrder = 3 }
                );
            });
        }

        /// <summary>
        /// Cập nhật CreatedDate và ModifiedDate
        /// </summary>
        private void UpdateTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entities)
            {
                if (entityEntry.Entity is IMLEntity entity)
                {
                    if (!entity.ModifiedDate.HasValue)
                    {
                        entity.ModifiedDate = DateTime.UtcNow;
                    }


                    if (entityEntry.State == EntityState.Added)
                    {
                        entity.CreatedDate = entity.ModifiedDate;
                    }
                }
            }
        }
    }
}
