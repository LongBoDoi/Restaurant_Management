import CommonFunction from '@/common/CommonFunction';
import { EnumReservationStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import EventName from '@/common/EventName';
import { MLActionResult, Reservation } from '@/models';
import { reservationService } from '@/services/reservationService';
import { defineStore } from 'pinia'

export const reservationStore = defineStore('reservationStore', {
  state: () => {
    return {
      _reservations: <Reservation[]>[],
      _totalCount: <number>0,
      _selectedIndex: <number>0,
    }
  },

  getters: {
    reservations(state) {
      return state._reservations;
    },

    totalCount(state) {
      return state._totalCount;
    },

    selectedIndex(state) {
      return state._selectedIndex;
    }
  },

  actions: {
    setSelectedIndex(index: number) {
      this._selectedIndex = index;
    },

    removeSelectedReservation() {
      this._reservations.splice(this._selectedIndex, 1);
      this._selectedIndex = 0;
    },

    /**
     * Lấy dữ liệu bài test
     */
    async getReservationsData(status: EnumReservationStatus, page: number, itemsPerPage: number) {
      try {
        const result:MLActionResult = await reservationService.getReservations(status, page, itemsPerPage);
        if (result.Success) {
          this._reservations = result.Data.Reservations as Reservation[];
          this._totalCount = result.Data.TotalCount ?? 0;

          this._selectedIndex = 0;
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