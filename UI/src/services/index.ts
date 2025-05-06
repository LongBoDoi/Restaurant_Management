import ChatbotService, { chatbotService } from "./chatbotService";
import CustomerService, { customerService } from "./customerService";
import InventoryItemService, { inventoryItemService } from "./inventoryItemService";
import MenuItemService, { menuItemService } from "./menuItemService";
import ReservationService, { reservationService } from "./reservationService";
import UserLoginService, { userLoginService } from "./userLoginService";
import EmployeeService, { employeeService } from "./employeeService";
import OrderService, { orderService } from "./orderService";
import SettingService, { settingService } from "./settingService";
import MenuItemCategoryService, { menuItemCategoryService } from "./menuItemCategoryService";
import InventoryItemCategoryService, { inventoryItemCategoryService } from "./inventoryItemCategoryService";
import TableService, { tableService } from "./tableService";
import AreaService, { areaService } from "./areaService";
import CustomMenuRequestService, { customMenuRequestService } from "./customMenuRequestService";
import DashboardService, { dashboardService } from "./dashboardService";
import RoleService, { roleService } from "./roleService";
import PermissionService, { permissionService } from "./permissionService";

class ServiceInterface {
    UserLoginService: UserLoginService = userLoginService;
    ChatbotService: ChatbotService = chatbotService;
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
    DashboardService: DashboardService = dashboardService;
    RoleService: RoleService = roleService;
    PermissionService: PermissionService = permissionService;
};

const configTokenForService = (token: any) => {
    userLoginService.setToken(token);
    chatbotService.setToken(token);
    reservationService.setToken(token);
    menuItemService.setToken(token);
    customerService.setToken(token);
    inventoryItemService.setToken(token);
    inventoryItemCategoryService.setToken(token);
    employeeService.setToken(token);
    orderService.setToken(token);
    settingService.setToken(token);
    menuItemCategoryService.setToken(token);
    tableService.setToken(token);
    areaService.setToken(token);
    customMenuRequestService.setToken(token);
    dashboardService.setToken(token);
    roleService.setToken(token);
    permissionService.setToken(token);
};

export { configTokenForService }

export default ServiceInterface;