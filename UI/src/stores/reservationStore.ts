import CommonFunction from '@/common/CommonFunction';
import { EnumReservationStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import EventName from '@/common/EventName';
import { MLActionResult, Reservation } from '@/models';
import { reservationService } from '@/services/reservationService';
import { createStoreOnBase } from './baseStore';

export const reservationStore = createStoreOnBase('reservationStore', reservationService, {
  actions: {
    /**
     * Lấy dữ liệu bài test
     */
    async getReservationsData(status: EnumReservationStatus, page: number, itemsPerPage: number) {
      try {
        const me = this as any;

        me._dataList = [];
        const result:MLActionResult = await reservationService.getReservations(status, page, itemsPerPage);
        if (result.Success) {
          me._dataList = result.Data.Data as Reservation[];
          me._totalCount = result.Data.TotalCount ?? 0;

          me._selectedIndex = 0;
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
})