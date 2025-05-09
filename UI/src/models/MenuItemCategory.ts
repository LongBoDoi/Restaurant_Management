import MenuItem from "./MenuItem";
import MLEntity from "./MLEntity";

interface MenuItemCategory extends MLEntity {
    MenuItemCategoryID: string,
    MenuItemCategoryName: string,
    Description: string,
    Inactive: boolean,
    SortOrder: number,
    Color: string,

    ItemCount: number,
    MenuItems: MenuItem[]
}

export default MenuItemCategory;