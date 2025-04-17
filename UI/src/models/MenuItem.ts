import MenuItemCategory from "./MenuItemCategory";
import MenuItemInventoryItem from "./MenuItemInventoryItem";
import MLEntity from "./MLEntity";
import OrderDetail from "./OrderDetail";

interface MenuItem extends MLEntity {
    MenuItemID: string,
    Name: string,
    Description: string,
    Price: number,
    MenuItemCategoryID: string,
    Inactive: boolean,
    ImageUrl: string,

    MenuItemCategory: MenuItemCategory,
    MenuItemInventoryItem: MenuItemInventoryItem[],
    OrderDetails: OrderDetail[]
};

export default MenuItem;