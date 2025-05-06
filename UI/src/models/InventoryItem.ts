import InventoryItemCategory from "./InventoryItemCategory";
import MLEntity from "./MLEntity";

interface InventoryItem extends MLEntity {
    InventoryItemID: string,
    Name: string,
    Quantity: number,
    WarningStockQuantity: number,
    Unit: string,
    InventoryItemCategoryID: string,
    Inactive: boolean,

    InventoryItemCategory?: InventoryItemCategory
};

export default InventoryItem;