import ChatbotService from "./chatbotService";
import CustomerService, { customerService } from "./customerService";
import InventoryItemService, { inventoryItemService } from "./inventoryItemService";
import MenuItemService, { menuItemService } from "./menuItemService";
import ReservationService, { reservationService } from "./reservationService";
import UserLoginService from "./userLoginService";
import EmployeeService, { employeeService } from "./employeeService";

class ServiceInterface {
    UserLoginService: UserLoginService = new UserLoginService;
    ChatbotService: ChatbotService = new ChatbotService;
    ReservationService: ReservationService = reservationService;
    MenuItemService: MenuItemService = menuItemService;
    CustomerService: CustomerService = customerService;
    InventoryItemService: InventoryItemService = inventoryItemService;
    EmployeeService: EmployeeService = employeeService;
};

export default ServiceInterface;