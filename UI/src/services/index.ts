import ChatbotService from "./chatbotService";
import CustomerService, { customerService } from "./customerService";
import MenuItemService, { menuItemService } from "./menuItemService";
import ReservationService, { reservationService } from "./reservationService";
import UserLoginService from "./userLoginService";

class ServiceInterface {
    UserLoginService: UserLoginService = new UserLoginService;
    ChatbotService: ChatbotService = new ChatbotService;
    ReservationService: ReservationService = reservationService;
    MenuItemService: MenuItemService = menuItemService;
    CustomerService: CustomerService = customerService;
};

export default ServiceInterface;