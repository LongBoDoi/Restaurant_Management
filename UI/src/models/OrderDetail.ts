import MenuItem from "./MenuItem";
import MLEntity from "./MLEntity";

interface OrderDetail extends MLEntity {
    OrderDetailID: string,
    OrderID: string,
    MenuItemID: string,
    Quantity: number,
    Price: number,
    Note: string,

    MenuItem?: MenuItem
};

export default OrderDetail;