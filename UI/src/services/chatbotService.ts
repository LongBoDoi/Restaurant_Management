import { ChatbotConversation, ChatbotConversationDetail } from "@/models";
import MLBaseService from "./baseService";
import { EnumChatbotSender } from "@/common/Enumeration";
import axios from "axios";
import CommonFunction from "@/common/CommonFunction";

const chatbotApi = axios.create({
    baseURL: `http://localhost:5005`
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

    // /**
    //  * Tạo đoạn chat mới
    //  */
    // async createNewConversation() : Promise<ChatbotConversation|undefined> {
    //     const response = await this.api.post('/CreateNewConversation');

    //     return response?.data?.Data;
    // }

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
    async getNewResponse(userID: string, message: string) : Promise<ChatbotConversationDetail[]> {
        try {
            const response = await chatbotApi.post('/webhooks/rest/webhook', {
                sender: userID,
                message: message
            });
            return response.data.map((item: any) => {
                return {
                    Sender: EnumChatbotSender.Bot,
                    Message: item.text
                }
            }) as ChatbotConversationDetail[];
        } catch (e) {
            CommonFunction.handleException(e);
        }
        return [];
    }
}

export default ChatbotService;
export const chatbotService = new ChatbotService();