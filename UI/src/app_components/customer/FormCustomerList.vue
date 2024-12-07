<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="height: calc(100vh - 64px); padding: 16px 32px 16px 16px; display: flex; flex-direction: column;">
        <VLabel class="flex-shrink-0" style="font-weight: bold; font-size: 2rem;">Khách hàng</VLabel>

        <VBtn width="fit-content" class="mt-4" color="primary" prepend-icon="mdi-plus" @click="handleAddNewClick">Thêm khách hàng</VBtn>

        <VSpacer style="height: 16px; flex-shrink: 0; flex-grow: 0;" />

        <MLVbox style="flex-grow: 1; overflow: hidden;">
            
        <VDataTableServer
            :items-length="totalCount"
            :loading="loading"
            loading-text="Đang tải dữ liệu..." 
            no-data-text="Không có dữ liệu" 
            items-per-page-text="Số bản ghi" 
            :headers="[
                {
                    title: 'Tên khách hàng',
                    value: 'CustomerName',
                },
                {
                    title: 'Số điện thoại',
                    value: 'PhoneNumber',
                    width: 250,
                },
                {
                    title: 'Địa chỉ',
                    value: 'Address',
                },
            ]"
            :items="(dataList as Customer[])"
            style="height: 100%;"
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
                <td>{{ item.CustomerName }}</td>
                <td>{{ item.PhoneNumber }}</td>
                <td>{{ item.Address }}</td>
              </tr>
            </template>
        </VDataTableServer>
        </MLVbox>
    </VSheet>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { Customer } from '@/models';
import { customerStore } from '@/stores/customerStore';
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
        ...mapActions(customerStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormCustomerDetail, newRecord);
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
            
            EventBus.emit(this.$eventName.ShowFormCustomerDetail, selectedRecord);
        }
    },

    computed: {
        ...mapState(customerStore, ['dataList', 'selectedIndex', 'totalCount']),

        tableHeaders():any {
            return [
                { title: 'Tên món', value: 'Name', width: 300, color: 'primary' },
                { title: 'Giá món', value: 'Price', align: 'end', width: 150 },
                { title: 'Mô tả', value: 'Description' },
                { title: 'Hết hàng', value: 'OutOfStock', align: 'center', width: 150 }
            ]
        }
    }
}
</script>

<style lang="scss" scoped>
.selected-row {
    background-color: #81c784;
    color: white;
}
</style>