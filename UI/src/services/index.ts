import ChatbotService from "./chatbotService";
import UserLoginService from "./userLoginService";

class ServiceInterface {
    UserLoginService: UserLoginService = new UserLoginService;
    ChatbotService: ChatbotService = new ChatbotService;
};

export default ServiceInterface;