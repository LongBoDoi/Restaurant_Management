import InventoryItem from "./InventoryItem";
import MLEntity from "./MLEntity";

interface InventoryItemCategory extends MLEntity {
    InventoryItemCategoryID: string,
    InventoryItemCategoryName: string,
    InventoryItemCount: number,
    Inactive: boolean,

    InventoryItems: InventoryItem[]
};

export default InventoryItemCategory;