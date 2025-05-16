import { ChatbotConversation, ChatbotConversationDetail } from "@/models";
import MLBaseService from "./baseService";
import axios from "axios";
import CommonFunction from "@/common/CommonFunction";

const chatbotApi = axios.create({
    baseURL: import.meta.env.VITE_CHATBOT_URL,
})

class ChatbotService extends MLBaseService<ChatbotConversation> {
    protected entityName: string = 'Chatbot';

    constructor() {
        super();
        this.configApi();
    }

    // /**
    //  * Lấy dữ liệu đoạn chat
    //  */
    // async getChatbotConversation(conversationID:string) : Promise<ChatbotConversation|undefined> {
    //     const response = await this.api.get('/GetChatbotConversation', {
    //         params: {
    //             conversationID: conversationID
    //         }
    //     });

    //     return response?.data?.Data;
    // }

    /**
     * Tạo đoạn chat mới
     */
    async createNewConversation() : Promise<ChatbotConversationDetail[]> {
        try {
            const timezone = CommonFunction.getTimeZone();
    
            const response = await chatbotApi.post('/createNewConversation', {
                timezone: timezone
            });
            
            if (response.status === 200) {
                return response.data;
            }
    
            return [];
        } catch (e) {
            return [];
        }
    }

    // /**
    //  * Gửi tin nhắn mới
    //  */
    // async sendNewMessage(conversationID:string, message:string):Promise<ChatbotConversationDetail|undefined> {
    //     // const detail:ChatbotConversationDetail = {
    //     //     ConversationID: conversationID,
    //     //     Sender: EnumChatbotSender.User,
    //     //     Message: message
    //     // } as ChatbotConversationDetail;

    //     // const response = await this.api.post('/SendChatbotMessage', detail);
    //     // return response?.data?.Data;

    //     const response = await chatbotApi.post('/webhooks/rest/webhook', {
    //         sender: "",
    //         message: message
    //     });

    //     return {
    //         Sender: EnumChatbotSender.User,
    //         Message: response.data.text
    //     } as ChatbotConversationDetail;
    // }

    /**
     * Lấy phản hồi từ chatbot
     */
    async getNewResponse(conversationDetails: ChatbotConversationDetail[], message: string) : Promise<ChatbotConversationDetail|undefined> {
        try {
            const response = await chatbotApi.post('/getNewResponse', {
                conversation: conversationDetails,
                message: message,
                timezone: CommonFunction.getTimeZone()
            });
            
            if (response.status === 200) {
                return response.data;
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }
        return undefined;
    }
}

export default ChatbotService;
export const chatbotService = new ChatbotService();