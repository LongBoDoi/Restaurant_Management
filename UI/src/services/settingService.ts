import { MenuItemCategory, MLActionResult, Setting } from "@/models";
import BaseService from "./baseService";
import CommonFunction from "@/common/CommonFunction";

class SettingService extends BaseService<Setting> {
    protected entityName: string = 'Setting';
    
    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy danh sách thiết lập
     */
    async getSettings() : Promise<Setting[]> {
        try {
            const response = await this.api.get('/GetSettings');
            
            const result = response?.data as MLActionResult;
            if (result) {
                if (result.Success) {
                    return response.data.Data as Setting[];
                }
                
                if (result.ErrorMsg) {
                    CommonFunction.showToastMessage(result.ErrorMsg, 'error');
                }
            }
            return [];
        } catch (e) {
            CommonFunction.handleException(e);
            return [];
        }
    }

    /**
     * Lấy danh sách món hiển thị trên màn hình của khách hàng
     */
    async getMenuItemsForCustomerScreen() : Promise<MenuItemCategory[]> {
        var result = [] as MenuItemCategory[];

        try {
            const response = await this.api.get('/GetMenuItemsForCustomerScreen');
            
            const actionResult = response?.data as MLActionResult;
            if (actionResult?.Success) {
                result = actionResult.Data as MenuItemCategory[];
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }

        return result;
    }

    /**
     * Lưu thiết lập
     */
    async updateSettings(lstSettings: Setting[], introImage:File|undefined, menuImages: File[]) : Promise<boolean> {
        var success:boolean = false;

        try {
            const formData = new FormData();
            formData.append("settings", JSON.stringify(lstSettings));
            if (introImage) {
                formData.append("introImage", introImage);
            }
            menuImages.forEach(menuImage => {
                formData.append('menuImages', menuImage);
            })

            const response = await this.api.post('/UpdateSettings', formData, {
                headers: {
                    "Content-Type": "multipart/form-data"
                }
            });
            
            const result = response?.data as MLActionResult;
            if (result) {
                success = result.Success;

                if (result.Success) {
                    CommonFunction.showToastMessage('Lưu thành công', 'success');
                } else if (result.ErrorMsg) {
                    CommonFunction.showToastMessage(result.ErrorMsg, 'error');
                }
            }
        } catch (e) {
            success = false;
            CommonFunction.handleException(e);
        }

        return success;
    }
}

export default SettingService;

const settingService = new SettingService();
export {settingService};