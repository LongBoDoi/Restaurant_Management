import MLBaseService from "./baseService";
import InventoryItem from "@/models/InventoryItem";

class InventoryItemService extends MLBaseService<InventoryItem> {
    protected entityName: string = 'InventoryItem';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default InventoryItemService;

const inventoryItemService = new InventoryItemService();
export {inventoryItemService};