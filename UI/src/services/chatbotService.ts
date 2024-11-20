import { ChatbotConversation, ChatbotConversationDetail } from "@/models";
import BaseService from "./baseService";
import { EnumChatbotSender } from "@/common/Enumeration";

class ChatbotService extends BaseService {
    protected entityName: string = 'Chatbot';
    
    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy dữ liệu đoạn chat
     */
    async getChatbotConversation(conversationID:string) : Promise<ChatbotConversation|undefined> {
        //FAKE DỮ LIỆU
        //#Todo: Sửa thành API đúng
        const conversation : ChatbotConversation|undefined = JSON.parse(sessionStorage.getItem('chatbotConversation') ?? '');
        return new Promise((resolve:any) => {
            setTimeout(() => {
                resolve(conversation);
            }, 1000);
        });
    }

    /**
     * Tạo đoạn chat mới
     */
    async createNewConversation() : Promise<ChatbotConversation|undefined> {
        //FAKE DỮ LIỆU
        //#Todo: Sửa thành API đúng
        const currentDate = new Date();
        const conversation = {
            ConversationID: '5474678c-7ae5-49a3-a88c-09ed105515ae',
            Topic: 'Đặt chỗ',
            ChatbotConversationDetails: [
                {
                    Sender: EnumChatbotSender.Bot,
                    Message: 'Xin chào. Tôi có thể giúp gì cho bạn?',
                    Timestamp: currentDate
                } as ChatbotConversationDetail
            ]
        } as ChatbotConversation;

        //#TODO: Xoá dòng này sau khi implement
        sessionStorage.setItem('chatbotConversation', JSON.stringify(conversation));

        return new Promise((resolve:any) => {
            setTimeout(() => {
                resolve(conversation);
            }, 1000);
        });
    }

    /**
     * Gửi tin nhắn mới
     */
    async sendNewMessage(conversationID:string, message:string):Promise<ChatbotConversationDetail|undefined> {
        //FAKE DỮ LIỆU
        //#Todo: Sửa thành API đúng
        const detail:ChatbotConversationDetail = {
            ConversationID: conversationID,
            Sender: EnumChatbotSender.User,
            Message: message,
            Timestamp: new Date()
        } as ChatbotConversationDetail;
        return new Promise((resolve:any) => {
            setTimeout(() => {
                resolve(detail);
            }, 1000);
        });
    }

    /**
     * Lấy phản hồi từ chatbot
     */
    async getNewResponse() : Promise<ChatbotConversationDetail|undefined> {
        const detail:ChatbotConversationDetail = {
            Sender: EnumChatbotSender.Bot,
            Message: 'Xin lỗi, tôi không hiểu.',
            Timestamp: new Date()
        } as ChatbotConversationDetail;
        return new Promise((resolve:any) => {
            setTimeout(() => {
                resolve(detail);
            }, 1000);
        });
    }
}

export default ChatbotService;