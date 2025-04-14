import InventoryItemCategory from "@/models/InventoryItemCategory";
import BaseService from "./baseService";
import { MLActionResult } from "@/models";
import CommonFunction from "@/common/CommonFunction";

class InventoryItemCategoryService extends BaseService<InventoryItemCategory> {
    protected entityName: string = 'InventoryItemCategory';
    
    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy danh sách nguyên liệu cho khách hàng tự tạo sushi
     */
    async getInventoryItemForCustomerCreate():Promise<InventoryItemCategory[]> {
        var result:InventoryItemCategory[] = [];
        
        try {
            const response = await this.api.get('GetInventoryItemForCustomerCreate');
            const actionResult = response.data as MLActionResult;
            if (actionResult.Success) {
                result = actionResult.Data as InventoryItemCategory[];
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }

        return result;
    }
}

export default InventoryItemCategoryService;

const inventoryItemCategoryService = new InventoryItemCategoryService();
export {inventoryItemCategoryService};