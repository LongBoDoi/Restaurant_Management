import ChatbotService from "./chatbotService";
import CustomerService, { customerService } from "./customerService";
import InventoryItemService, { inventoryItemService } from "./inventoryItemService";
import MenuItemService, { menuItemService } from "./menuItemService";
import ReservationService, { reservationService } from "./reservationService";
import UserLoginService from "./userLoginService";
import EmployeeService, { employeeService } from "./employeeService";
import OrderService, { orderService } from "./orderService";
import SettingService, { settingService } from "./settingService";
import MenuItemCategoryService, { menuItemCategoryService } from "./menuItemCategoryService";

class ServiceInterface {
    UserLoginService: UserLoginService = new UserLoginService;
    ChatbotService: ChatbotService = new ChatbotService;
    ReservationService: ReservationService = reservationService;
    MenuItemService: MenuItemService = menuItemService;
    CustomerService: CustomerService = customerService;
    InventoryItemService: InventoryItemService = inventoryItemService;
    EmployeeService: EmployeeService = employeeService;
    OrderService: OrderService = orderService;
    SettingService: SettingService = settingService;
    MenuItemCategoryService: MenuItemCategoryService = menuItemCategoryService;
};

export default ServiceInterface;