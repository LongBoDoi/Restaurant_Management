import { MenuItem, MLActionResult } from "@/models";
import BaseService from "./baseService";

class MenuItemService extends BaseService {
    protected entityName: string = 'MenuItem';
    
    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy dữ liệu đoạn chat
     */
    async getMenuItems(page: number, itemsPerPage: number) : Promise<MLActionResult> {
        const response = await this.api.get('/GetMenuItems', {
            params: {
                page: page,
                itemsPerPage: itemsPerPage
            }
        });
        return response.data as MLActionResult;
    }

    /**
     * Lấy dữ liệu đoạn chat
     */
    async updateMenuItemInfo(menuItem:MenuItem) : Promise<MLActionResult> {
        const response = await this.api.post('/UpdateMenuItemInfo', menuItem);
        return response.data as MLActionResult;
    }
}

export default MenuItemService;

const menuItemService = new MenuItemService();
export {menuItemService};