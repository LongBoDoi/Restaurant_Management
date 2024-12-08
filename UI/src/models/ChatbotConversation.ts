import ChatbotConversationDetail from "./ChatbotConversationDetail";
import MLEntity from "./MLEntity";

interface ChatbotConversation extends MLEntity {
    /// Khoá chính
    ConversationID: string,

    /// ID khách hàng
    CustomerID?: string,

    /// Chủ đề
    Topic: string,

    /// Có hữu ích với người dùng không
    IsHelpful: boolean,

    /// Danh sách tin nhắn trong cuộc hội thoại
    ChatbotConversationDetails: ChatbotConversationDetail[]
}

export default ChatbotConversation;