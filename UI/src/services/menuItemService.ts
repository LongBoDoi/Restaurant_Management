import { MenuItem } from "@/models";
import BaseService from "./baseService";

class MenuItemService extends BaseService<MenuItem> {
    protected entityName: string = 'MenuItem';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default MenuItemService;

const menuItemService = new MenuItemService();
export {menuItemService};