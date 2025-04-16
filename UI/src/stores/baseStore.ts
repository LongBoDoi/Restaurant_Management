import { EnumEditMode } from "@/common/Enumeration";
import MLEntity from "@/models/MLEntity";
import PagingData from "@/models/PagingData";
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

            removeSelectedRecord() {
                this._dataList.splice(this._selectedIndex, 1);
                this._selectedIndex = Math.min(this.selectedIndex, this._dataList.length - 1);
            },

            addNewRecord():IMLEntity {
                const newRecord = {
                    EditMode: EnumEditMode.Add
                } as any;

                this._dataList.unshift(newRecord);
                this._selectedIndex = this._dataList.indexOf(newRecord);

                return newRecord;
            },

            async getDataPaging(page: number, itemsPerPage: number, search?: string, filter?: string) {
                const pagingData:PagingData<IMLEntity> = await service.getDataPaging(page, itemsPerPage, search, filter);
                
                this._dataList = pagingData.Data;
                this._totalCount = pagingData.TotalCount;
      
                this._selectedIndex = 0;
            },

            ...config?.actions
        },
    });
}