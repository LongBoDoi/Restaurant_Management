import { EnumChatbotSender } from "@/common/Enumeration";
import ChatbotConversation from "./ChatbotConversation";

interface ChatbotConversationDetail {
    /// Khoá chính
    ConversationDetailID: string,

    /// ID đoạn hội thoại
    ConversationID: string,

    /// Người gửi
    Sender: EnumChatbotSender,

    /// Tin nhắn
    Message: string,

    /// Thời gian gửi
    Timestamp: Date,

    /// Dữ liệu đoạn hội thoại
    ChatbotConversation?: ChatbotConversation
}

export default ChatbotConversationDetail;