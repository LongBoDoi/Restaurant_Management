import MenuItemCategory from "./MenuItemCategory";
import MLEntity from "./MLEntity";

interface MenuItem extends MLEntity {
    MenuItemID: string,
    Name: string,
    Description: string,
    Price: number,
    MenuItemCategoryID: string,
    OutOfStock: boolean,
    ImageUrl: string,

    MenuItemCategory?: MenuItemCategory
};

export default MenuItem;