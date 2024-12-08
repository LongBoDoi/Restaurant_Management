import { ChatbotConversation, ChatbotConversationDetail } from "@/models";
import BaseService from "./baseService";
import { EnumChatbotSender } from "@/common/Enumeration";
import axios from "axios";

class ChatbotService extends BaseService<ChatbotConversation> {
    protected entityName: string = 'Chatbot';

    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy dữ liệu đoạn chat
     */
    async getChatbotConversation(conversationID:string) : Promise<ChatbotConversation|undefined> {
        const response = await this.api.get('/GetChatbotConversation', {
            params: {
                conversationID: conversationID
            }
        });

        return response?.data?.Data;
    }

    /**
     * Tạo đoạn chat mới
     */
    async createNewConversation() : Promise<ChatbotConversation|undefined> {
        const response = await this.api.post('/CreateNewConversation');

        return response?.data?.Data;
    }

    /**
     * Gửi tin nhắn mới
     */
    async sendNewMessage(conversationID:string, message:string):Promise<ChatbotConversationDetail|undefined> {
        const detail:ChatbotConversationDetail = {
            ConversationID: conversationID,
            Sender: EnumChatbotSender.User,
            Message: message
        } as ChatbotConversationDetail;

        const response = await this.api.post('/SendChatbotMessage', detail);
        return response?.data?.Data;
    }

    /**
     * Lấy phản hồi từ chatbot
     */
    async getNewResponse(conversationID: string, message: string) : Promise<ChatbotConversationDetail|undefined> {
        //#TODO: Thay đầu API của chat bot
        const chatbotApiUrl = 'http://localhost:9000/GetNewChatbotResponse';
        const response = await axios.get(chatbotApiUrl, {
            //Truyền param ở đây
            params: {
                conversationID: conversationID,
                message: message,
            }
        });

        return response?.data.Data;
    }
}

export default ChatbotService;