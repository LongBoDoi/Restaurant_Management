
interface ChatbotConversationDetail {
    role: 'system'|'assistant'|'user',
    content: string,
    timestamp: Date
}

export default ChatbotConversationDetail;