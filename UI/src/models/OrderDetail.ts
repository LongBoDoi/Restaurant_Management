import MenuItem from "./MenuItem";
import MLEntity from "./MLEntity";

interface OrderDetail extends MLEntity {
    OrderDetailID: string,
    OrderID: string,
    MenuItemID?: string,
    MenuItemName: string,
    Quantity: number,
    Price: number,
    Amount: number,
    Note: string,

    MenuItem?: MenuItem
};

export default OrderDetail;