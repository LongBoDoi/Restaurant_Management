import { menuItemService } from '@/services/menuItemService';
import { createStoreOnBase } from './baseStore';
import { MenuItem } from '@/models';
import PagingData from '@/models/PagingData';
 
export const menuItemStore = createStoreOnBase<MenuItem>('menuItemStore', menuItemService, {
    actions: {
        async getMenuItemPaging(page: number, itemsPerPage: number, search: string, categoryID: string) {
            const pagingData:PagingData<MenuItem> = await menuItemService.getMenuItemPaging(page, itemsPerPage, search, categoryID);
            
            const me = this as any;
            me._dataList = pagingData.Data;
            me._totalCount = pagingData.TotalCount;
        },
    }
});