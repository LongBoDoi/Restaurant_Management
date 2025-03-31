import InventoryItem from "./InventoryItem";
import MenuItem from "./MenuItem";

interface MenuItemInventoryItem {
    MenuItemID: string,
    MenuItem?: MenuItem,

    InventoryItemID: string,
    InventoryItem?: InventoryItem,

    Quantity: number
}

export default MenuItemInventoryItem;