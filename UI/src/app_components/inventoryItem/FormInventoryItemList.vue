<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="height: 100%; padding: 16px 32px 16px 16px; display: flex; flex-direction: column;">
        <VLabel style="font-weight: bold; font-size: 2rem;">Nguyên liệu</VLabel>

        <VBtn width="fit-content" class="mt-4" color="primary" prepend-icon="mdi-plus" @click="handleAddNewClick">Thêm nguyên liệu</VBtn>

        <VSpacer style="height: 16px; flex-shrink: 0; flex-grow: 0;" />

        <VDataTableServer
            :items-length="totalCount"
            :loading="loading"
            loading-text="Đang tải dữ liệu..." 
            no-data-text="Không có dữ liệu" 
            items-per-page-text="Số bản ghi" 
            :headers="[
                {
                    title: 'Tên nguyên liệu',
                    value: 'Name',
                },
                {
                    title: 'Số lượng tồn',
                    value: 'Quantity',
                    width: 250,
                },
                {
                    title: 'Đơn vị',
                    value: 'Unit',
                },
            ]"
            :items="(dataList as InventoryItem[])"
            style="flex-grow: 1;"
            :items-per-page-options="[10, 25, 50, 100]"
            :hover="true"
            v-model:options="options"
            @update:options="getMenuItems"
        >
            <template v-slot:item="{ item, index }">
              <tr v-if="item !== null" style="cursor: pointer;" :class="{'selected-row': index === selectedIndex}" @click="setSelectedIndex(index)" @dblclick="handleOpenFormDetail">
                <!-- <td>
                  <v-checkbox
                    :value="item"
                    hide-details
                    color="primary"
                  ></v-checkbox>
                </td> -->
                <td>{{ item.Name }}</td>
                <td>{{ item.Quantity }}</td>
                <td>{{ item.Unit }}</td>
              </tr>
            </template>
        </VDataTableServer>
    </VSheet>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { InventoryItem } from '@/models';
import { inventoryItemStore } from '@/stores/inventoryItemStore';
import { mapActions, mapState } from 'pinia';

export default {
    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10
            }
        }
    },

    methods: {
        ...mapActions(inventoryItemStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormInventoryItemDetail, newRecord);
        },

        /**
         * Lấy danh sách món
         */
        async getMenuItems() {
            this.loading = true;
            await this.getDataPaging(this.options.page, this.options.itemsPerPage);
            this.loading = false;
        },

        /**
         * Xử lý mở form chi tiết khách hàng
         */
        handleOpenFormDetail() {
            const selectedRecord = this.dataList[this.selectedIndex];
            selectedRecord.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormInventoryItemDetail, selectedRecord);
        }
    },

    computed: {
        ...mapState(inventoryItemStore, ['dataList', 'selectedIndex', 'totalCount']),
    }
}
</script>

<style lang="scss" scoped>
.selected-row {
    background-color: #81c784;
    color: white;
}
</style>