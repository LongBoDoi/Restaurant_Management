import MLEntity from "./MLEntity";

interface InventoryItem extends MLEntity {
    InventoryItemID: string,
    Name: string,
    Quantity: number,
    Unit: string
};

export default InventoryItem;