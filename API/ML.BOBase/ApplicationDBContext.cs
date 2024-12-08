using API.ML.BO;
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

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasOne(ul => ul.Employee).WithOne(e => e.UserLogin).HasForeignKey<UserLogin>(ul => ul.EmployeeID);
                entity.HasOne(ul => ul.Customer).WithOne(c => c.UserLogin).HasForeignKey<UserLogin>(ul => ul.CustomerID);
            });

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
