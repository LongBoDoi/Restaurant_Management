import CommonFunction from '@/common/CommonFunction';
import EventBus from '@/common/EventBus';
import EventName from '@/common/EventName';
import { MenuItem, MLActionResult } from '@/models';
import { menuItemService } from '@/services/menuItemService';
import { defineStore } from 'pinia'

export const menuItemStore = defineStore('menuItemStore', {
  state: () => {
    return {
      _menuItems: <MenuItem[]>[],
      _totalCount: <number>0,
      _selectedIndex: <number>0,
    }
  },

  getters: {
    menuItems(state) {
      return state._menuItems;
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
      this._menuItems.splice(this._selectedIndex, 1);
      this._selectedIndex = 0;
    },

    /**
     * Lấy dữ liệu bài test
     */
    async getMenuItemsData(page: number, itemsPerPage: number) {
      try {
        const result:MLActionResult = await menuItemService.getMenuItems(page, itemsPerPage);
        if (result.Success) {
          this._menuItems = result.Data.MenuItems as MenuItem[];
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