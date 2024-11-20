/**
 * Enum người gửi tin nhắn trong chat bot
 */
export enum EnumChatbotSender {
    /// Bot
    Bot = 0,

    /// Người dùng
    User = 1
}

class EnumerationInterface {
    EnumChatbotSender: typeof EnumChatbotSender = EnumChatbotSender;
}

export default EnumerationInterface;