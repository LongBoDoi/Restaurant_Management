import { MenuItem, MLActionResult } from "@/models";
import BaseService from "./baseService";
import PagingData from "@/models/PagingData";
import CommonFunction from "@/common/CommonFunction";

class MenuItemService extends BaseService<MenuItem> {
  protected entityName: string = 'MenuItem';
  
  constructor() {
      super();
      this.configApi();
  }

  public async getMenuItemPaging(page: number, itemsPerPage: number, search: string) : Promise<PagingData<MenuItem>> {
    var result:PagingData<MenuItem> = {
      Data: [],
      TotalCount: 0
    };

    try {
      const response = await this.api.get('/GetMenuItemPaging', {
        params: {
          page: page,
          itemsPerPage: itemsPerPage,
          search: search
        }
      });
      const actionResult = response.data as MLActionResult;

      if (actionResult?.Success) {
        result = actionResult.Data as PagingData<MenuItem>;
      } else if (actionResult?.ErrorMsg) {
        CommonFunction.showToastMessage(actionResult.ErrorMsg, 'error');
      }
    } catch (e) {
      CommonFunction.handleException(e);
    }

    return result;
  }

  /**
   * Lưu món
   */
  async updateMenuItem(menuItem: MenuItem, image:File|undefined) : Promise<MLActionResult> {
    var result:MLActionResult = {
      Success: false
    };

    try {
      const formData = new FormData();
      formData.append("menuItem", JSON.stringify(menuItem));
      if (image) {
          formData.append("image", image);
      }

      const response = await this.api.post('/UpdateMenuItem', formData, {
        headers: {
            "Content-Type": "multipart/form-data"
        }
      });
      result = response.data as MLActionResult;
    } catch (e) {
      CommonFunction.handleException(e);
    }

    return result;
  }
}

export default MenuItemService;

const menuItemService = new MenuItemService();
export {menuItemService};