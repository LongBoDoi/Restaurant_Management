import { EnumEditMode, EnumMenuItemCategory } from "@/common/Enumeration";

interface MenuItem {
    MenuItemID: string,
    Name: string,
    Description: string,
    Price: number,
    Category: EnumMenuItemCategory|null,
    OutOfStock: boolean,

    EditMode?: EnumEditMode
};

export default MenuItem;