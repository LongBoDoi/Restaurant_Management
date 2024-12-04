using API.ML.BOBase;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class ChatbotConversation : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid ConversationID { get; set; }

        /// <summary>
        /// ID khách hàng
        /// </summary>
        public Guid? CustomerID { get; set; }

        /// <summary>
        /// Chủ đề hỏi
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// Cuộc hội thoại có hữu ích không
        /// </summary>
        public bool IsHelpful { get; set; }

        /// <summary>
        /// Dữ liệu tin nhắn hội thoại
        /// </summary>
        public IEnumerable<ChatbotConversationDetail>? ChatbotConversationDetails { get; set; }

        /// <summary>
        /// Dữ liệu khách hàng tạo đoạn hội thoại
        /// </summary>
        public Customer? Customer { get; set; }
    }
}
