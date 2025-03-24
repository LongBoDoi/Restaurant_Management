import { MenuItemCategory } from "@/models";
import BaseService from "./baseService";

class MenuItemCategoryService extends BaseService<MenuItemCategory> {
  protected entityName: string = 'MenuItemCategory';
  
  constructor() {
      super();
      this.configApi();
  }
}

export default MenuItemCategoryService;

const menuItemCategoryService = new MenuItemCategoryService();
export {menuItemCategoryService};