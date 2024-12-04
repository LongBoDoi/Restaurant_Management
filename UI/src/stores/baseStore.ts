import CommonFunction from "@/common/CommonFunction";
import { EnumEditMode } from "@/common/Enumeration";
import EventBus from "@/common/EventBus";
import EventName from "@/common/EventName";
import { MLActionResult } from "@/models";
import MLEntity from "@/models/MLEntity";
import BaseService from "@/services/baseService";
import { defineStore } from "pinia";

export function createStoreOnBase<IMLEntity extends MLEntity>(storeID: string, service:BaseService<any>, config?: any) {
    return defineStore(storeID, {
        state: () => {
            return {
                _dataList: <IMLEntity[]>[],
                _totalCount: <number>0,
                _selectedIndex: <number>0,
                ...config?.state
            }
        },
        getters: {
            dataList(state:any) {
                return state._dataList;
            },

            totalCount(state:any) {
                return state._totalCount;
            },

            selectedIndex(state:any) {
                return state._selectedIndex;
            },

            options(state:any) {
                return state._options;
            },

            ...config?.getters
        },
        actions: {
            setSelectedIndex(index: number) {
                this._selectedIndex = index;
            },

            removeSelectedReservation() {
                this._dataList.splice(this._selectedIndex, 1);
                this._selectedIndex = 0;
            },

            addNewRecord():IMLEntity {
                const newRecord = {
                    EditMode: EnumEditMode.Add
                } as any;

                this._dataList.push(newRecord);
                this._selectedIndex = this._dataList.indexOf(newRecord);

                return newRecord;
            },

            async getDataPaging(page: number, itemsPerPage: number) {
                try {
                    const result:MLActionResult = await service.getDataPaging(page, itemsPerPage);
                    if (result.Success) {
                      this._dataList = result.Data.Data;
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

            ...config?.actions
        },
    });
}