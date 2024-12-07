import { EnumMenuItemCategory } from "@/common/Enumeration";
import MLEntity from "./MLEntity";

interface MenuItem extends MLEntity {
    MenuItemID: string,
    Name: string,
    Description: string,
    Price: number,
    Category: EnumMenuItemCategory,
    OutOfStock: boolean,
};

export default MenuItem;