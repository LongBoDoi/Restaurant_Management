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
import InventoryItemCategoryService, { inventoryItemCategoryService } from "./inventoryItemCategoryService";
import TableService, { tableService } from "./tableService";
import AreaService, { areaService } from "./areaService";
import CustomMenuRequestService, { customMenuRequestService } from "./customMenuRequestService";

class ServiceInterface {
    UserLoginService: UserLoginService = new UserLoginService;
    ChatbotService: ChatbotService = new ChatbotService;
    ReservationService: ReservationService = reservationService;
    MenuItemService: MenuItemService = menuItemService;
    CustomerService: CustomerService = customerService;
    InventoryItemService: InventoryItemService = inventoryItemService;
    InventoryItemCategoryService: InventoryItemCategoryService = inventoryItemCategoryService;
    EmployeeService: EmployeeService = employeeService;
    OrderService: OrderService = orderService;
    SettingService: SettingService = settingService;
    MenuItemCategoryService: MenuItemCategoryService = menuItemCategoryService;
    TableService: TableService = tableService;
    AreaService: AreaService = areaService;
    CustomMenuRequestService: CustomMenuRequestService = customMenuRequestService;
};

export default ServiceInterface;