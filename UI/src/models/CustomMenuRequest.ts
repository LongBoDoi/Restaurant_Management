import { EnumCustomMenuRequestStatus } from "@/common/Enumeration";
import MLEntity from "./MLEntity";
import Customer from "./Customer";
import InventoryItem from "./InventoryItem";

interface CustomMenuRequest extends MLEntity {
    CustomMenuRequestID: string,
    CustomerID: string,
    CustomerName: string,
    MenuItemName: string,
    Status: EnumCustomMenuRequestStatus,
    Note: string,
    Price: number,

    Customer?: Customer,
    InventoryItems?: InventoryItem[],

    SaveToOrder: boolean,
    OrderID?: string,
    SaveToMenu: boolean,
}

export default CustomMenuRequest;