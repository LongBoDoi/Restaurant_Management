import { EnumEditMode } from '@/common/Enumeration';
import { createStoreOnBase } from './baseStore';
import { MenuItemCategory } from '@/models';
import PagingData from '@/models/PagingData';
import { menuItemCategoryService } from '@/services/menuItemCategoryService';
import CommonValue from '@/common/CommonValue';
 
export const menuItemCategoryStore = createStoreOnBase<MenuItemCategory>('menuItemCategoryStore', menuItemCategoryService, {
    state: {
        _selectedIndex: <string>''
    },
    actions: {
        setSelectedIndex(index: string) {
            (this as any)._selectedIndex = index;
        },

        addNewRecord():MenuItemCategory {
            const me:any = this;
            const newRecord = {
                MenuItemCategoryID: CommonValue.GuidEmpty,
                EditMode: EnumEditMode.Add
            } as MenuItemCategory;

            me._dataList.push(newRecord);
            me._selectedIndex = CommonValue.GuidEmpty;

            return newRecord;
        },

        async getDataPaging(page: number, itemsPerPage: number, search: string) {
            const me:any = this;
            const pagingData:PagingData<MenuItemCategory> = await menuItemCategoryService.getDataPaging(page, itemsPerPage, search);
            
            me._dataList = pagingData.Data as MenuItemCategory[];
            me._totalCount = pagingData.TotalCount;
  
            me._selectedIndex = (me._dataList[0] as MenuItemCategory)?.MenuItemCategoryID ?? '';
        },
    }
});