import { orderService } from '@/services/orderService';
import { createStoreOnBase } from './baseStore';
import { MLActionResult, Order } from '@/models';
import CommonFunction from '@/common/CommonFunction';
import EventBus from '@/common/EventBus';
import EventName from '@/common/EventName';
import { EnumOrderStatus } from '@/common/Enumeration';

export const orderStore = createStoreOnBase<Order>('orderStore', orderService, {
    actions: {
        async getOrdersByStatus(status: EnumOrderStatus, page: number, itemsPerPage: number) {
            try {
                (this as any)._dataList = [];

                const result:MLActionResult = await orderService.getOrdersByStatus(status, page, itemsPerPage);
                if (result.Success) {
                  (this as any)._dataList = result.Data.Data;
                  (this as any)._totalCount = result.Data.TotalCount ?? 0;
        
                  (this as any)._selectedIndex = 0;
                } else {
                  EventBus.emit(EventName.ShowToastMessage, {
                      Message: result.ErrorMsg,
                      Type: 'error'
                  });
                }
            } catch (e) {
                CommonFunction.handleException(e);
            }
        },
    }
});