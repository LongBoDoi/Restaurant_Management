using API.ML.BOBase;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class Customer : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid CustomerID { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Sở thích
        /// </summary>
        public string? Preferences { get; set; }

        /// <summary>
        /// Điểm tích luỹ
        /// </summary>
        public int LoyaltyPoint { get; set; }

        /// <summary>
        /// Danh sách cuộc hội thoại với chatbot
        /// </summary>
        public IEnumerable<ChatbotConversation>? ChatbotConversations { get; set; }

        /// <summary>
        /// Danh sách bản ghi đặt bàn
        /// </summary>
        public IEnumerable<Reservation>? Reservations { get; set; }

        /// <summary>
        /// Thông tin đăng nhập
        /// </summary>
        public UserLogin? UserLogin { get; set; }

        /// <summary>
        /// Danh sách bản ghi order
        /// </summary>
        public IEnumerable<Order>? Orders { get; set; }
    }
}
