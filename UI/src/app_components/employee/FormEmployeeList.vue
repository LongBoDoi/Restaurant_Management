<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="height: calc(100vh - 64px); padding: 16px 32px 16px 16px; display: flex; flex-direction: column;">
        <VLabel class="flex-shrink-0" style="font-weight: bold; font-size: 2rem;">Nhân viên</VLabel>

        <VBtn width="fit-content" class="mt-4" color="primary" prepend-icon="mdi-plus" @click="handleAddNewClick">Thêm nhân viên</VBtn>

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
                    title: 'Mã nhân viên',
                    value: 'EmployeeCode',
                    width: 250,
                },
                {
                    title: 'Tên nhân viên',
                    value: 'EmployeeName',
                },
                {
                    title: 'Số điện thoại',
                    value: 'PhoneNumber',
                    width: 250,
                },
                {
                    title: 'Vai trò',
                    value: 'Role',
                    width: 250
                },
            ]"
            :items="(dataList as Employee[])"
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
                <td>{{ item.EmployeeCode }}</td>
                <td>{{ item.EmployeeName }}</td>
                <td>{{ item.PhoneNumber }}</td>
                <td>{{ getRoleName(item.Role) }}</td>
              </tr>
            </template>
        </VDataTableServer>
        </MLVbox>
    </VSheet>
</template>

<script lang="ts">
import { EnumRole } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Employee } from '@/models';
import { employeeStore } from '@/stores/employeeStore';
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
        ...mapActions(employeeStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormEmployeeDetail, newRecord);
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
            
            EventBus.emit(this.$eventName.ShowFormEmployeeDetail, selectedRecord);
        },

        getRoleName(role: EnumRole) {
            switch (role) {
                case EnumRole.Manager:
                    return 'Quản lý';
                case EnumRole.Cashier:
                    return 'Thu ngân';
                case EnumRole.Waiter:
                    return 'Phục vụ';
            }
        }
    },

    computed: {
        ...mapState(employeeStore, ['dataList', 'selectedIndex', 'totalCount']),
    }
}
</script>

<style lang="scss" scoped>
.selected-row {
    background-color: #81c784;
    color: white;
}
</style>