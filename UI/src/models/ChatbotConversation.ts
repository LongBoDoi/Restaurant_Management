import ChatbotConversationDetail from "./ChatbotConversationDetail";

interface ChatbotConversation {
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