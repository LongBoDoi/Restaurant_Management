import { MenuItemCategory, MLActionResult } from "@/models";
import BaseService from "./baseService";
import PagingData from "@/models/PagingData";
import CommonFunction from "@/common/CommonFunction";

class MenuItemCategoryService extends BaseService<MenuItemCategory> {
  protected entityName: string = 'MenuItemCategory';
  
  constructor() {
      super();
      this.configApi();
  }

  /**
   * Lấy dữ liệu phân trang nhóm thực đơn
   * @param page Số trang
   * @param itemsPerPage Kích thước trang
   * @param search Tìm kiếm (Tên)
   */
  public async getMenuItemCategoryPaging(page: number, itemsPerPage: number, search: string) : Promise<PagingData<MenuItemCategory>> {
    var result:PagingData<MenuItemCategory> = {
      Data: [],
      TotalCount: 0
    };

    try {
      const response = await this.api.get('/GetMenuItemCategoryPaging', {
        params: {
          page: page,
          itemsPerPage: itemsPerPage,
          search: search
        }
      });
      const actionResult = response.data as MLActionResult;

      if (actionResult?.Success) {
        result = actionResult.Data as PagingData<MenuItemCategory>;
      }
    } catch (e) {
      CommonFunction.handleException(e);
    }

    return result;
  }
}

export default MenuItemCategoryService;

const menuItemCategoryService = new MenuItemCategoryService();
export {menuItemCategoryService};