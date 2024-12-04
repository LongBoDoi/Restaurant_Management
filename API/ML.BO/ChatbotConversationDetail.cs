using API.ML.Common;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class ChatbotConversationDetail
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid ConversationDetailID { get; set; }

        /// <summary>
        /// ID đoạn hội thoại
        /// </summary>
        public Guid ConversationID { get; set; }

        /// <summary>
        /// Người gửi
        /// </summary>
        public EnumChatbotSender Sender { get; set; }

        /// <summary>
        /// Nội dung tin nhắn
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian gửi
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Dữ liệu cuộc hội thoại
        /// </summary>
        public ChatbotConversation? ChatbotConversation { get; set; }
    }
}
